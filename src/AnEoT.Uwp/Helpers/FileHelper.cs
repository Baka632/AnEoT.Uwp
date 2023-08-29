using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Windows.Storage;

namespace AnEoT.Uwp.Helpers;

/// <summary>
/// 为访问应用包中的文件提供帮助方法的类
/// </summary>
internal static class FileHelper
{
    /// <summary>
    /// 获取指定期刊的封面
    /// </summary>
    /// <param name="volumeRawName">期刊原始名称</param>
    /// <returns>指向指定期刊的 <see cref="Uri"/></returns>
    public static async Task<Uri> GetVolumeCover(string volumeRawName)
    {
        string baseUri = $"ms-appx:///Assets/Test/posts/{volumeRawName}/";

        string readmeFileUri = $"{baseUri}/README.md";
        StorageFile readmeFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(readmeFileUri));

        string readmeMarkdown = await FileIO.ReadTextAsync(readmeFile);
        MarkdownDocument doc = Markdown.Parse(readmeMarkdown, MarkdownHelper.Pipeline);

        LinkInline imgLink = doc.Descendants<LinkInline>().First();

        Uri coverUri = new(new Uri(baseUri, UriKind.Absolute), imgLink.Url);
        return coverUri;
    }

    /// <summary>
    /// 获取指定期刊的目录页
    /// </summary>
    /// <param name="volumeRawName">期刊原始名称</param>
    /// <returns>指定期刊目录页的 Markdown 字符串</returns>
    public static async Task<string> GetVolumeReadmeMarkdown(string volumeRawName)
    {
        string readmeFileUri = $"ms-appx:///Assets/Test/posts/{volumeRawName}/README.md";
        StorageFile readmeFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(readmeFileUri));
        string readmeMarkdown = await FileIO.ReadTextAsync(readmeFile);

        return readmeMarkdown;
    }
}
