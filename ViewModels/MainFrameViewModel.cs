using System.Threading.Tasks;
using Windows.Storage;

namespace AnEoT.Uwp.ViewModels;

/// <summary>
/// <seealso cref="MainFrame"/> 的视图模型
/// </summary>
public sealed class MainFrameViewModel : NotificationObject
{
    /// <summary>
    /// 获取表示磁贴的 XML
    /// </summary>
    /// <param name="fileName">XML 文件名</param>
    /// <returns>表示磁贴的 XML</returns>
    /// <exception cref="ArgumentException"><paramref name="fileName"/>为 null 或空白</exception>
    public async Task<string> GetTileXml(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException($"“{nameof(fileName)}”不能为 null 或空白。", nameof(fileName));
        }

        StorageFolder tileFolder = await App.AssetsFolder.GetFolderAsync("Tile");
        StorageFile tileXml = await tileFolder.GetFileAsync(fileName);
        string xml = await FileIO.ReadTextAsync(tileXml);
        return xml;
    }
}
