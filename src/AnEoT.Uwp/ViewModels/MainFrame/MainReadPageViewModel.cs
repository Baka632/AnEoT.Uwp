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

        string[] splitedTitle = info.Name.Split(new char[] { '：', ':' }, StringSplitOptions.RemoveEmptyEntries);

        AdaptiveTileBuilder builder = new();
        builder.ConfigureDisplayName("最新一期");
        builder.TileWide
            .AddBackgroundImage("https://aneot.terrach.net/hero/3.webp", 50)
            .AddAdaptiveText(splitedTitle[0], true, AdaptiveTextStyle.Base);

        if (splitedTitle.Length > 1)
        {
            //主题刊
            //我们在这里将主题名称单列一行
            builder.TileWide.AddAdaptiveText(splitedTitle[1], true, AdaptiveTextStyle.Base);
        }

        UpdateTile(builder.BuildXml(), lastestVolumeTile);
    }

    public void CreateDefaultTileAsync(PreviewTile tile)
    {
        AdaptiveTileBuilder builder = new();
        builder.TileSmall
            .ConfigureTextStacking(TileTextStacking.Center)
            .AddAdaptiveText("🤔", hintStyle: AdaptiveTextStyle.Subtitle, hintAlign: AdaptiveTextAlign.Center);
        builder.TileMedium
            .ConfigureTextStacking(TileTextStacking.Center)
            .AddAdaptiveText("🤔正在构思...", hintStyle: AdaptiveTextStyle.Caption, hintAlign: AdaptiveTextAlign.Center);
        builder.TileWide
            .ConfigureTextStacking(TileTextStacking.Center)
            .AddAdaptiveText("🤔正在构思...", hintStyle: AdaptiveTextStyle.Title, hintAlign: AdaptiveTextAlign.Center);
        builder.TileLarge
            .ConfigureTextStacking(TileTextStacking.Center)
            .AddAdaptiveText("🤔", hintStyle: AdaptiveTextStyle.Header, hintAlign: AdaptiveTextAlign.Center)
            .AddAdaptiveText("正在构思...", hintStyle: AdaptiveTextStyle.Subheader, hintAlign: AdaptiveTextAlign.Center);

        UpdateTile(builder.BuildXml(), tile);
    }

    public void CreateRssTileAsync(PreviewTile rssTile)
    {
        rssTile.VisualElements.BackgroundColor = (Color)XamlBindingHelper.ConvertValue(typeof(Color), "#fb9f0b");

        AdaptiveTileBuilder builder = new();
        builder.TileSmall
            .ConfigureTextStacking(TileTextStacking.Center)
            .AddPeekImage("ms-appx:///Assets/Images/rss.png", hintOverlay: 0)
            .AddAdaptiveText("订阅", hintAlign: AdaptiveTextAlign.Center);
        builder.TileMedium
            .ConfigureTextStacking(TileTextStacking.Center)
            .AddPeekImage("ms-appx:///Assets/Images/rss.png", hintOverlay: 0)
            .AddAdaptiveText("订阅《回归线》", hintAlign: AdaptiveTextAlign.Center);
        builder.TileWide
            .ConfigureTextStacking(TileTextStacking.Center)
            .AddPeekImage("ms-appx:///Assets/Images/rss.png", hintOverlay: 0)
            .AddAdaptiveText("订阅《回归线》", hintStyle: AdaptiveTextStyle.Subtitle, hintAlign: AdaptiveTextAlign.Center);
        builder.TileLarge
            .ConfigureTextStacking(TileTextStacking.Center)
            .AddPeekImage("ms-appx:///Assets/Images/rss.png", hintOverlay: 0)
            .AddAdaptiveText("订阅《回归线》", hintStyle: AdaptiveTextStyle.Subtitle, hintAlign: AdaptiveTextAlign.Center);

        UpdateTile(builder.BuildXml(), rssTile);
    }

    public void CreateWelcomeTileAsync(PreviewTile welcomeTile)
    {
        AdaptiveTileBuilder builder = new();
        builder.ConfigureDisplayName("欢迎");
        builder.TileWide
            .AddBackgroundImage("ms-appx:///Assets/Images/Welcome.jpg")
            .AddAdaptiveText("卷首", true, AdaptiveTextStyle.Subtitle)
            .AddAdaptiveText("欢迎阅读《回归线》", true);

        UpdateTile(builder.BuildXml(), welcomeTile);
    }

    private void UpdateTile(XmlDocument xmlDocument, PreviewTile tile)
    {
        TileNotification notification = new(xmlDocument);
        tile.CreateTileUpdater().Update(notification);
    }
}
