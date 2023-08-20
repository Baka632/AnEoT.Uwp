using System.Threading.Tasks;
using AnEoT.Uwp.Contracts;
using AnEoT.Uwp.Services;
using Microsoft.Toolkit.Uwp.Notifications;
using NotificationsVisualizerLibrary;
using Windows.ApplicationModel;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Markup;

namespace AnEoT.Uwp.ViewModels.MainFrame;

/// <summary>
/// <seealso cref="Views.MainFrame.MainReadPage"/> 的视图模型
/// </summary>
public sealed class MainReadPageViewModel : NotificationObject
{
    private readonly StorageFolder assetsFolder;
    private readonly IVolumeProvider provider;

    public MainReadPageViewModel()
    {
        assetsFolder = Package.Current.InstalledLocation.GetFolderAsync("Assets").AsTask().Result;
        StorageFolder testFolder = assetsFolder.GetFolderAsync("Test").AsTask().Result;
        StorageFolder postsFolder = testFolder.GetFolderAsync("posts").AsTask().Result;

        provider = new FileVolumeProvider(postsFolder.Path);
    }

    public async Task CreateLatestVolumeTileAsync(PreviewTile lastestVolumeTile)
    {
        VolumeInfo info = await provider.GetLatestVolumeInfoAsync();

        string[] strs = info.Name.Split(new char[] { '：', ':' }, StringSplitOptions.RemoveEmptyEntries);

        AdaptiveTileBuilder builder = new();
        builder.ConfigureDisplayName("最新一期");
        builder.TileWide
            .SetBackgroundImage("https://aneot.terrach.net/hero/3.webp", 50)
            .AddAdaptiveText(strs[0], true, AdaptiveTextStyle.Base);

        if (strs.Length > 1)
        {
            builder.TileWide.AddAdaptiveText(strs[1], true, AdaptiveTextStyle.Base);
        }

        TileContent tileContent = builder.Build();

        UpdateTile(tileContent.GetXml(), lastestVolumeTile);
    }

    public async Task CreateDefaultTileAsync(PreviewTile tile)
    {
        XmlDocument content = await TileHelper.GetTileXmlDocument("Working.xml");
        UpdateTile(content, tile);
    }

    public async Task CreateRssTileAsync(PreviewTile rssTile)
    {
        rssTile.VisualElements.BackgroundColor = (Color)XamlBindingHelper.ConvertValue(typeof(Color), "#fb9f0b");

        XmlDocument content = await TileHelper.GetTileXmlDocument("RssTile.xml");
        UpdateTile(content, rssTile);
    }

    public async Task CreateWelcomeTileAsync(PreviewTile welcomeTile)
    {
        XmlDocument content = await TileHelper.GetTileXmlDocument("Welcome.xml");
        UpdateTile(content, welcomeTile);
    }

    private void UpdateTile(XmlDocument xmlDocument, PreviewTile tile)
    {
        TileNotification notification = new(xmlDocument);
        tile.CreateTileUpdater().Update(notification);
    }
}
