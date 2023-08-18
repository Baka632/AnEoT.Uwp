using System.Threading.Tasks;
using AnEoT.Uwp.Contracts;
using Markdig;
using Windows.Storage;
using Windows.Storage.Search;

namespace AnEoT.Uwp.Services;

/// <summary>
/// 基于本地文件的期刊获取器
/// </summary>
public sealed class FileVolumeProvider : IVolumeProvider
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

    public async Task<VolumeDetail> GetVolumeAsync(string volume)
    {
        StorageFolder baseFolder = await StorageFolder.GetFolderFromPathAsync(CurrentPath);
        StorageFolder volumeFolder = await baseFolder.GetFolderAsync(volume);
        return await GetVolumeDetailFromStorageFolderAsync(volumeFolder);
    }

    public async Task<VolumeInfo> GetVolumeInfoAsync(string volume)
    {
        StorageFolder baseFolder = await StorageFolder.GetFolderFromPathAsync(CurrentPath);
        StorageFolder volumeFolder = await baseFolder.GetFolderAsync(volume);
        return await GetVolumeInfoFromStorageFolderAsync(volumeFolder);
    }

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

        string volumeName = null;
        List<ArticleDetail> articles = new(fileList.Count());

        foreach (StorageFile file in fileList)
        {
            string markdown = await FileIO.ReadTextAsync(file);

            if (MarkdownHelper.TryGetFromFrontMatter(markdown, out MarkdownArticleInfo result))
            {
                if (file.Name == "README.md")
                {
                    volumeName = result.Title;
                }
                else
                {

                    if (DateTimeOffset.TryParse(result.Date, out DateTimeOffset date) != true)
                    {
                        date = new DateTimeOffset();
                    }

                    ArticleDetail articleDetail = new(result.Title, result.Author, result.Description, date, markdown,
                                                      result.Category, result.Tag);
                    articles.Add(articleDetail);
                }
            }
        }

        if (volumeName is not null && articles.Any())
        {
            VolumeDetail detail = new(volumeName, articles);
            return detail;
        }
        else
        {
            throw new ArgumentException("使用指定的参数，无法获取指定期刊信息");
        }
    }

    private static async Task<VolumeInfo> GetVolumeInfoFromStorageFolderAsync(StorageFolder volumeFolder)
    {
        StorageFile file = await volumeFolder.GetFileAsync("README.md");
        string markdown = await FileIO.ReadTextAsync(file);

        if (MarkdownHelper.TryGetFromFrontMatter(markdown, out MarkdownArticleInfo result))
        {
            VolumeInfo volumeInfo = new(result.Title);

            return volumeInfo;
        }
        else
        {
            throw new ArgumentException("使用指定参数，找到的 README.md 文件无效");
        }
    }
}
