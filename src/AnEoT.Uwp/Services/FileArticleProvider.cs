using AnEoT.Uwp.Contracts;
using AnEoT.Uwp.Models.Markdown;
using Windows.Storage;
using Windows.Storage.Search;

namespace AnEoT.Uwp.Services;

/// <summary>
/// 基于本地文件的文章获取器
/// </summary>
public readonly struct FileArticleProvider : IArticleProvider
{
    /// <summary>
    /// 当前使用的文件夹路径
    /// </summary>
    public string CurrentPath { get; }

    /// <summary>
    /// 使用指定的参数构造 <see cref="FileArticleProvider"/> 的新实例
    /// </summary>
    /// <param name="path">包含文章的文件夹路径，将直接从这里开始搜索文章</param>
    public FileArticleProvider(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException($"“{nameof(path)}”不能为 null 或空白。", nameof(path));
        }

        CurrentPath = path;
    }

    /// <inheritdoc/>
    public async Task<ArticleDetail> GetArticleAsync(string volume, string article)
    {
        StorageFolder baseFolder = await StorageFolder.GetFolderFromPathAsync(CurrentPath);
        StorageFolder volumeFolder = await GetVolumeFolder(volume, baseFolder);

        return await GetArticleDetailFromStorageFolderAsync(volumeFolder, article);
    }

    /// <inheritdoc/>
    public async Task<ArticleInfo> GetArticleInfoAsync(string volume, string article)
    {
        StorageFolder baseFolder = await StorageFolder.GetFolderFromPathAsync(CurrentPath);
        StorageFolder volumeFolder = await GetVolumeFolder(volume, baseFolder);

        return await GetArticleInfoFromStorageFolderAsync(volumeFolder, article);
    }

    private async Task<ArticleDetail> GetArticleDetailFromStorageFolderAsync(StorageFolder volumeFolder, string article)
    {
        StorageFile file = (await volumeFolder.GetFilesAsync(CommonFileQuery.OrderByName))
                    .FirstOrDefault(file =>
                    {
                        return file.Name.EndsWith(".md", StringComparison.OrdinalIgnoreCase) &&
                                            string.Equals(file.DisplayName, article, StringComparison.OrdinalIgnoreCase);
                    });

        if (file is not null)
        {
            string markdown = await FileIO.ReadTextAsync(file);

            if (MarkdownHelper.TryGetFromFrontMatter(markdown, out MarkdownArticleInfo result))
            {
                if (DateTimeOffset.TryParse(result.Date, out DateTimeOffset date) != true)
                {
                    date = new DateTimeOffset();
                }

                string description = string.IsNullOrWhiteSpace(result.Description) ? MarkdownHelper.GetArticleQuote(markdown) : result.Description;
                ArticleDetail articleDetail = new(result.Title, result.Author ?? "Another end of Terra", description.Trim(), date, markdown,
                                                  result.Category, result.Tag, result.Order, result.ShortTitle);
                return articleDetail;
            }
        }

        throw new ArgumentException("使用指定的参数，无法获取指定文章");
    }

    private async Task<ArticleInfo> GetArticleInfoFromStorageFolderAsync(StorageFolder volumeFolder, string article)
    {
        StorageFile file = (await volumeFolder.GetFilesAsync(CommonFileQuery.OrderByName))
                    .FirstOrDefault(file =>
                    {
                        return file.Name.EndsWith(".md", StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(file.DisplayName, article, StringComparison.OrdinalIgnoreCase);
                    });

        if (file is not null)
        {
            if (string.Equals(file.DisplayName, article, StringComparison.OrdinalIgnoreCase))
            {
                string markdown = await FileIO.ReadTextAsync(file);

                if (MarkdownHelper.TryGetFromFrontMatter(markdown, out MarkdownArticleInfo result))
                {
                    if (DateTimeOffset.TryParse(result.Date, out DateTimeOffset date) != true)
                    {
                        date = new DateTimeOffset();
                    }

                    string description = string.IsNullOrWhiteSpace(result.Description) ? MarkdownHelper.GetArticleQuote(markdown) : result.Description;
                    ArticleInfo articleInfo = new(result.Title, result.Author ?? "Another end of Terra", description.Trim(), date,
                                                      result.Category, result.Tag, result.Order, result.ShortTitle);
                    return articleInfo;
                }
            }
        }

        throw new ArgumentException("使用指定的参数，无法获取指定文章的信息");
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
