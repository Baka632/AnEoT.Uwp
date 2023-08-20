using System.Threading.Tasks;
using AnEoT.Uwp.ViewModels.MainFrame;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace AnEoT.Uwp.Views.MainFrame;

/// <summary>
/// 可用于自身或导航至 Frame 内部的空白页。
/// </summary>
public sealed partial class MainReadPage : Page
{
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
        await ViewModel.CreateDefaultTileAsync(FavoriteTile);
        await ViewModel.CreateDefaultTileAsync(VolumeListTile);
        await ViewModel.CreateDefaultTileAsync(HistoryTile);

        await ViewModel.CreateWelcomeTileAsync(WelcomeTile);
        await ViewModel.CreateRssTileAsync(RSSTile);
        await ViewModel.CreateLatestVolumeTileAsync(LastestVolumeTile);
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
}
