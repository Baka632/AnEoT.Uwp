using AnEoT.Uwp.Contracts;
using AnEoT.Uwp.Helpers.Comparer;
using AnEoT.Uwp.Models.Markdown;
using Windows.Storage;
using Windows.Storage.Search;

namespace AnEoT.Uwp.Services;

/// <summary>
/// 基于本地文件的期刊获取器
/// </summary>
public readonly struct FileVolumeProvider : IVolumeProvider
{
    /// <summary>
    /// 当前使用的文件夹路径
    /// </summary>
    public string CurrentPath { get; }

    /// <summary>
    /// 使用指定的参数构造 <see cref="FileVolumeProvider"/> 的新实例
    /// </summary>
    /// <param name="path">包含期刊文件的文件夹路径，将直接从这里开始搜索期刊</param>
    public FileVolumeProvider(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException($"“{nameof(path)}”不能为 null 或空白。", nameof(path));
        }

        CurrentPath = path;
    }

    /// <inheritdoc/>
    public async Task<VolumeDetail> GetVolumeAsync(string volume)
    {
        StorageFolder baseFolder = await StorageFolder.GetFolderFromPathAsync(CurrentPath);
        StorageFolder volumeFolder = await GetVolumeFolder(volume,baseFolder);
        return await GetVolumeDetailFromStorageFolderAsync(volumeFolder);
    }

    /// <inheritdoc/>
    public async Task<VolumeInfo> GetVolumeInfoAsync(string volume)
    {
        StorageFolder baseFolder = await StorageFolder.GetFolderFromPathAsync(CurrentPath);
        StorageFolder volumeFolder = await GetVolumeFolder(volume, baseFolder);
        return await GetVolumeInfoFromStorageFolderAsync(volumeFolder);
    }

    /// <inheritdoc/>
    public async Task<VolumeDetail> GetLatestVolumeAsync()
    {
        StorageFolder baseFolder = await StorageFolder.GetFolderFromPathAsync(CurrentPath);
        StorageFolder volumeFolder = (await baseFolder.GetFoldersAsync()).OrderBy(file => file.DisplayName).Reverse().FirstOrDefault();

        if (volumeFolder is null)
        {
            throw new ArgumentException("使用指定的参数，找不到期刊");
        }

        return await GetVolumeDetailFromStorageFolderAsync(volumeFolder);
    }

    /// <inheritdoc/>
    public async Task<VolumeInfo> GetLatestVolumeInfoAsync()
    {
        StorageFolder baseFolder = await StorageFolder.GetFolderFromPathAsync(CurrentPath);
        StorageFolder volumeFolder = (await baseFolder.GetFoldersAsync()).OrderBy(file => file.DisplayName).Reverse().FirstOrDefault();

        if (volumeFolder is null)
        {
            throw new ArgumentException("使用指定的参数，找不到期刊");
        }

        return await GetVolumeInfoFromStorageFolderAsync(volumeFolder);
    }

    private static async Task<VolumeDetail> GetVolumeDetailFromStorageFolderAsync(StorageFolder volumeFolder)
    {
        IEnumerable<StorageFile> fileList = (await volumeFolder.GetFilesAsync(CommonFileQuery.OrderByName))
                    .Where(file => file.Name.EndsWith(".md", StringComparison.OrdinalIgnoreCase));

        string volumeTitle = null;
        List<ArticleDetail> articles = new(fileList.Count());

        foreach (StorageFile file in fileList)
        {
            string markdown = await FileIO.ReadTextAsync(file);

            if (MarkdownHelper.TryGetFromFrontMatter(markdown, out MarkdownArticleInfo result))
            {
                if (file.Name.Equals("README.md", StringComparison.OrdinalIgnoreCase))
                {
                    volumeTitle = result.Title;
                }
                else
                {
                    if (DateTimeOffset.TryParse(result.Date, out DateTimeOffset date) != true)
                    {
                        date = new DateTimeOffset();
                    }

                    string description = string.IsNullOrWhiteSpace(result.Description) ? MarkdownHelper.GetArticleQuote(markdown) : result.Description;
                    ArticleDetail articleDetail = new(result.Title, result.Author ?? "Another end of Terra", description.Trim(), date, markdown,
                                                      result.Category, result.Tag, result.Order, result.ShortTitle);
                    articles.Add(articleDetail);
                }
            }
        }

        articles.Sort(new ArticleDetailOrInfoComparer());

        if (volumeTitle is not null && articles.Any())
        {
            VolumeDetail detail = new(volumeTitle, volumeFolder.DisplayName, articles);
            return detail;
        }
        else
        {
            throw new ArgumentException("使用指定的参数，无法获取指定期刊");
        }
    }

    private static async Task<VolumeInfo> GetVolumeInfoFromStorageFolderAsync(StorageFolder volumeFolder)
    {
        IEnumerable<StorageFile> fileList = (await volumeFolder.GetFilesAsync(CommonFileQuery.OrderByName))
                    .Where(file => file.Name.EndsWith(".md", StringComparison.OrdinalIgnoreCase));

        string volumeTitle = null;
        List<ArticleInfo> articles = new(fileList.Count());

        foreach (StorageFile file in fileList)
        {
            string markdown = await FileIO.ReadTextAsync(file);

            if (MarkdownHelper.TryGetFromFrontMatter(markdown, out MarkdownArticleInfo result))
            {
                if (file.Name.Equals("README.md", StringComparison.OrdinalIgnoreCase))
                {
                    volumeTitle = result.Title;
                }
                else
                {
                    if (DateTimeOffset.TryParse(result.Date, out DateTimeOffset date) != true)
                    {
                        date = new DateTimeOffset();
                    }

                    string description = string.IsNullOrWhiteSpace(result.Description) ? MarkdownHelper.GetArticleQuote(markdown) : result.Description;
                    ArticleInfo articleInfo = new(result.Title, result.Author ?? "Another end of Terra", description.Trim(), date,
                                                      result.Category, result.Tag, result.Order, result.ShortTitle);
                    articles.Add(articleInfo);
                }
            }
        }

        articles.Sort(new ArticleDetailOrInfoComparer());

        if (volumeTitle is not null && articles.Any())
        {
            VolumeInfo volumeInfo = new(volumeTitle, volumeFolder.DisplayName, articles);
            return volumeInfo;
        }
        else
        {
            throw new ArgumentException("使用指定的参数，无法获取指定期刊的信息");
        }
    }

    private static async Task<StorageFolder> GetVolumeFolder(string volume, StorageFolder baseFolder)
    {
        string volumeFolderPath = Path.Combine(baseFolder.Path, volume);
        if (Directory.Exists(volumeFolderPath) != true)
        {
            throw new DirectoryNotFoundException($"路径 {volumeFolderPath} 不存在");
        }

        return await baseFolder.GetFolderAsync(volume);
    }
}
