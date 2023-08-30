using AnEoT.Uwp.Contracts;
using AnEoT.Uwp.Models.Navigation;
using AnEoT.Uwp.Services;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.Toolkit.Uwp.Notifications;
using NotificationsVisualizerLibrary;
using Windows.ApplicationModel;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media.Animation;

namespace AnEoT.Uwp.ViewModels.MainFrame;

/// <summary>
/// <seealso cref="Views.MainFrame.MainReadPage"/> 的视图模型
/// </summary>
public sealed class MainReadPageViewModel : NotificationObject
{
    private readonly StorageFolder assetsFolder;
    private readonly IVolumeProvider volumeProvider;

    public DelegateCommand GoToRssSiteCommand { get; }
    public DelegateCommand GoToWelcomeArticleCommand { get; }
    public DelegateCommand GoToLatestVolumeCommand { get; }

    public MainReadPageViewModel()
    {
        assetsFolder = Package.Current.InstalledLocation.GetFolderAsync("Assets").AsTask().Result;
        StorageFolder testFolder = assetsFolder.GetFolderAsync("Test").AsTask().Result;
        StorageFolder postsFolder = testFolder.GetFolderAsync("posts").AsTask().Result;

        volumeProvider = new FileVolumeProvider(postsFolder.Path);

        GoToRssSiteCommand = new DelegateCommand(async obj =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new("https://aneot.terrach.net/subscription.html"));
        });

        GoToWelcomeArticleCommand = new DelegateCommand(obj =>
        {
            NavigationHelper.Navigate(typeof(ReadPage), new ArticleNavigationInfo("2022-06", "intro"), new DrillInNavigationTransitionInfo());
        });

        GoToLatestVolumeCommand = new DelegateCommand(async obj =>
        {
            VolumeDetail info = await volumeProvider.GetLatestVolumeAsync();
            NavigationHelper.Navigate(typeof(VolumePage), info.RawName, new DrillInNavigationTransitionInfo());
        });
    }

    public async Task<IEnumerable<XmlDocument>> GetLatestVolumeTilesAsync()
    {
        VolumeDetail info = await volumeProvider.GetLatestVolumeAsync();
        List<XmlDocument> xmlDocuments = new(info.Articles.Count());

        {
            string[] splitedTitle = info.Name.Split(new char[] { '：', ':' }, StringSplitOptions.RemoveEmptyEntries);
            AdaptiveTileBuilder mainTileBuilder = new();
            mainTileBuilder.ConfigureDisplayName("最新一期");
            mainTileBuilder.TileLarge
                .AddBackgroundImage("https://aneot.terrach.net/hero/3.webp", 50)
                .AddAdaptiveText(splitedTitle[0], true, AdaptiveTextStyle.Base);

            if (splitedTitle.Length > 1)
            {
                //主题刊
                //我们在这里将主题名称单列一行
                mainTileBuilder.TileLarge.AddAdaptiveText(splitedTitle[1], true, AdaptiveTextStyle.Base);
            }

            xmlDocuments.Add(mainTileBuilder.BuildXml());
        }

        foreach (ArticleDetail article in info.Articles)
        {
            MarkdownDocument markdownDoc = Markdown.Parse(article.MarkdownContent, MarkdownHelper.Pipeline);
            LinkInline img = markdownDoc.Descendants<LinkInline>().FirstOrDefault(link => link.IsImage);

            AdaptiveTileBuilder tileBuilder = new();

            // HACK: 考虑使用其他方式获得图片Uri，而不是写死
            if (img is not null)
            {
                string imgUri = $"ms-appx:///Assets/Test/posts/{info.RawName}/{img.Url}";
                tileBuilder.TileLarge.AddBackgroundImage(imgUri, 60);
            }

            tileBuilder.ConfigureDisplayName("最新一期");
            tileBuilder.TileLarge
                .AddAdaptiveText(article.Title, true);

            if (string.IsNullOrWhiteSpace(article.Description) != true)
            {
                tileBuilder.TileLarge.AddAdaptiveText(MarkdownHelper.ToPlainText(article.Description), true, AdaptiveTextStyle.CaptionSubtle);
            }

            xmlDocuments.Add(tileBuilder.BuildXml());
        }

        return xmlDocuments;
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
            .AddAdaptiveText("🤔正在构思...", hintStyle: AdaptiveTextStyle.Subtitle, hintAlign: AdaptiveTextAlign.Center);
        builder.TileLarge
            .ConfigureTextStacking(TileTextStacking.Center)
            .AddAdaptiveText("🤔", hintStyle: AdaptiveTextStyle.Header, hintAlign: AdaptiveTextAlign.Center)
            .AddAdaptiveText("正在构思...", hintStyle: AdaptiveTextStyle.Subheader, hintAlign: AdaptiveTextAlign.Center);

        UpdateTile(builder.BuildXml(), tile);
    }

    public void CreateRssTileAsync(PreviewTile rssTile)
    {
        rssTile.VisualElements.BackgroundColor = (Color)XamlBindingHelper.ConvertValue(typeof(Color), "#64fb9f0b");
        TileContent content = new()
        {
            Visual = new TileVisual()
            {
                DisplayName = "订阅《回归线》",
                TileWide = new TileBinding()
                {
                    Content = new TileBindingContentAdaptive()
                    {
                        Children =
                        {
                            new AdaptiveImage()
                            {
                                Source = "ms-appx:///Assets/Images/rss.png",
                                HintRemoveMargin = false
                            }
                        }
                    }
                }
            }
        };

        //AdaptiveTileBuilder builder = new();
        //builder.TileSmall
        //    .ConfigureTextStacking(TileTextStacking.Center)
        //    .AddPeekImage("ms-appx:///Assets/Images/rss.png", hintOverlay: 0)
        //    .AddAdaptiveText("订阅", hintAlign: AdaptiveTextAlign.Center);
        //builder.TileMedium
        //    .ConfigureTextStacking(TileTextStacking.Center)
        //    .AddPeekImage("ms-appx:///Assets/Images/rss.png", hintOverlay: 0)
        //    .AddAdaptiveText("订阅《回归线》", hintAlign: AdaptiveTextAlign.Center);
        //builder.TileWide
        //    .ConfigureTextStacking(TileTextStacking.Center)
        //    .AddPeekImage("ms-appx:///Assets/Images/rss.png", hintOverlay: 0)
        //    .AddAdaptiveText("订阅《回归线》", hintStyle: AdaptiveTextStyle.Subtitle, hintAlign: AdaptiveTextAlign.Center);
        //builder.TileLarge
        //    .ConfigureTextStacking(TileTextStacking.Center)
        //    .AddPeekImage("ms-appx:///Assets/Images/rss.png", hintOverlay: 0)
        //    .AddAdaptiveText("订阅《回归线》", hintStyle: AdaptiveTextStyle.Subtitle, hintAlign: AdaptiveTextAlign.Center);

        UpdateTile(content.GetXml(), rssTile);
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
