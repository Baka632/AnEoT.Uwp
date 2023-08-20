using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Data.Xml.Dom;
using Windows.Storage;

namespace AnEoT.Uwp.Helpers.Tile;

/// <summary>
/// 为磁贴相关操作提供帮助的类
/// </summary>
public static class TileHelper
{
    /// <summary>
    /// 获取表示磁贴的 XML 字符串
    /// </summary>
    /// <param name="fileName">XML 文件名</param>
    /// <returns>表示磁贴的 XML 字符串</returns>
    /// <exception cref="ArgumentException"><paramref name="fileName"/>为 null 或空白</exception>
    public static async Task<string> GetTileXmlString(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException($"“{nameof(fileName)}”不能为 null 或空白。", nameof(fileName));
        }

        StorageFolder assetsFolder = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
        StorageFolder tileFolder = await assetsFolder.GetFolderAsync("Tile");
        StorageFile tileXml = await tileFolder.GetFileAsync(fileName);
        string xml = await FileIO.ReadTextAsync(tileXml);
        return xml;
    }

    /// <summary>
    /// 获取表示磁贴的 <see cref="XmlDocument"/>
    /// </summary>
    /// <param name="fileName">XML 文件名</param>
    /// <returns>表示磁贴的 <see cref="XmlDocument"/></returns>
    /// <exception cref="ArgumentException"><paramref name="fileName"/>为 null 或空白</exception>
    public static async Task<XmlDocument> GetTileXmlDocument(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException($"“{nameof(fileName)}”不能为 null 或空白。", nameof(fileName));
        }

        XmlDocument doc = new();
        doc.LoadXml(await GetTileXmlString(fileName));
        return doc;
    }
}
