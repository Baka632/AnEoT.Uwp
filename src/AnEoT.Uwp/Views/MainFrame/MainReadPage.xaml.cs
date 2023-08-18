using System.Threading.Tasks;
using AnEoT.Uwp.Services;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Markup;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace AnEoT.Uwp.Views.MainFrame;

/// <summary>
/// 可用于自身或导航至 Frame 内部的空白页。
/// </summary>
public sealed partial class MainReadPage : Page
{
    public MainReadPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        SetupTile();
    }

    private async void SetupTile()
    {
        WelcomeTile.CreateTileUpdater().Update(new TileNotification(await TileHelper.GetTileXmlDocument("Welcome.xml")));
        RSSTile.VisualElements.BackgroundColor = (Color)XamlBindingHelper.ConvertValue(typeof(Color), "#fb9f0b");
        RSSTile.CreateTileUpdater().Update(new TileNotification(await TileHelper.GetTileXmlDocument("RssTile.xml")));

        FavoriteTile.CreateTileUpdater().Update(new TileNotification(await TileHelper.GetTileXmlDocument("Working.xml")));
        VolumeListTile.CreateTileUpdater().Update(new TileNotification(await TileHelper.GetTileXmlDocument("Working.xml")));
        HistoryTile.CreateTileUpdater().Update(new TileNotification(await TileHelper.GetTileXmlDocument("Working.xml")));
        await CreateLatestVolumeTile();
    }

    private async Task CreateLatestVolumeTile()
    {
        StorageFolder assetsFolder = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
        StorageFolder testFolder = await assetsFolder.GetFolderAsync("Test");
        StorageFolder postsFolder = await testFolder.GetFolderAsync("posts");

        FileVolumeProvider provider = new(postsFolder.Path);

        VolumeInfo info = await provider.GetLatestVolumeInfoAsync();

        string[] strs = info.Name.Split(new char[] { '：', ':' }, StringSplitOptions.RemoveEmptyEntries);

        TileContent tileContent = new()
        {
            Visual = new TileVisual()
            {
                DisplayName = "最新一期",
                TileWide = new TileBinding()
                {
                    Content = new TileBindingContentAdaptive()
                    {
                        BackgroundImage = new TileBackgroundImage()
                        {
                            Source = "https://aneot.terrach.net/hero/3.webp",
                            HintOverlay = 50
                        }
                    }
                }
            }
        };

        TileBindingContentAdaptive content = (TileBindingContentAdaptive)tileContent.Visual.TileWide.Content;

        if (strs.Length > 1)
        {
            AdaptiveText title = new()
            {
                Text = strs[0],
                HintStyle = AdaptiveTextStyle.Base,
                HintWrap = true
            };

            AdaptiveText theme = new()
            {
                Text = strs[1],
                HintStyle = AdaptiveTextStyle.Base,
                HintWrap = true
            };

            content.Children.Add(title);
            content.Children.Add(theme);
        }
        else
        {
            AdaptiveText title = new()
            {
                Text = strs[0],
                HintStyle = AdaptiveTextStyle.Base,
                HintWrap = true
            };
            content.Children.Add(title);
        }

        TileNotification tileNotif = new(tileContent.GetXml());
        LastestVolumeTile.CreateTileUpdater().Update(tileNotif);
    }

    private async void OnWelcomeTileClicked(object sender, RoutedEventArgs args)
    {
        Uri uri = new("https://aneot.terrach.net/posts/2022-06/intro.html");
        await Windows.System.Launcher.LaunchUriAsync(uri);
    }

    private void ScrollViewer_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.R)
        {
            SetupTile();
        }
    }

    private async void OnRssTileClicked(object sender, RoutedEventArgs args)
    {
        Uri uri = new("https://aneot.terrach.net/subscription.html");
        await Windows.System.Launcher.LaunchUriAsync(uri);
    }
}
