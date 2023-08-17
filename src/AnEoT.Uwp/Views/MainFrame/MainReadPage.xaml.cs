using Windows.Data.Xml.Dom;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.ViewManagement;

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

        FavoriteTile.CreateTileUpdater().Update(new TileNotification(await TileHelper.GetTileXmlDocument("Working.xml")));
        VolumeListTile.CreateTileUpdater().Update(new TileNotification(await TileHelper.GetTileXmlDocument("Working.xml")));
        HistoryTile.CreateTileUpdater().Update(new TileNotification(await TileHelper.GetTileXmlDocument("Working.xml")));
    }
}
