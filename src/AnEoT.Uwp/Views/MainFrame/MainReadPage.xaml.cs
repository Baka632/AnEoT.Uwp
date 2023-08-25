using System.Threading.Tasks;
using AnEoT.Uwp.ViewModels.MainFrame;
using NotificationsVisualizerLibrary;
using Windows.Data.Xml.Dom;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace AnEoT.Uwp.Views.MainFrame;

/// <summary>
/// 可用于自身或导航至 Frame 内部的空白页。
/// </summary>
public sealed partial class MainReadPage : Page
{
    private int _latestVolumeTileContentIndex = 0;
    private bool _isFirstShowOfLatestVolumeTile = true;
    private XmlDocument[] _latestVolumeTileContents;

    public MainReadPageViewModel ViewModel { get; } = new();

    public MainReadPage()
    {
        this.InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        await SetupTile();
    }

    private async Task SetupTile()
    {
        ViewModel.CreateDefaultTileAsync(FavoriteTile);
        ViewModel.CreateDefaultTileAsync(VolumeListTile);
        ViewModel.CreateDefaultTileAsync(HistoryTile);

        ViewModel.CreateWelcomeTileAsync(WelcomeTile);
        ViewModel.CreateRssTileAsync(RSSTile);

        _latestVolumeTileContents = (await ViewModel.GetLatestVolumeTilesAsync()).ToArray();
        _latestVolumeTileContentIndex = 0;
        await SetLatestVolumeTile(_latestVolumeTileContents.First());
    }

    private async void OnWelcomeTileClicked(object sender, RoutedEventArgs args)
    {
        Uri uri = new("https://aneot.terrach.net/posts/2022-06/intro.html");
        await Windows.System.Launcher.LaunchUriAsync(uri);
    }

    private async void ScrollViewer_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.R)
        {
            await SetupTile();
        }
    }

    private async void OnRssTileClicked(object sender, RoutedEventArgs args)
    {
        Uri uri = new("https://aneot.terrach.net/subscription.html");
        await Windows.System.Launcher.LaunchUriAsync(uri);
    }

    private async void OnLatestVolumeTileNewAnimationCompleted(object sender, RoutedEventArgs e)
    {
        _latestVolumeTileContentIndex++;

        if (_latestVolumeTileContents.Length <= _latestVolumeTileContentIndex)
        {
            _latestVolumeTileContentIndex = 0;
        }

        await SetLatestVolumeTile(_latestVolumeTileContents[_latestVolumeTileContentIndex]);
    }

    private async Task SetLatestVolumeTile(XmlDocument doc)
    {
        if (_isFirstShowOfLatestVolumeTile)
        {
            _isFirstShowOfLatestVolumeTile = false;
        }
        else
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
        }

        PreviewTileUpdater tileUpdater = LastestVolumeTile.CreateTileUpdater();
        tileUpdater.Update(new Windows.UI.Notifications.TileNotification(doc));
    }
}
