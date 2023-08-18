using System;
using Windows.Data.Xml.Dom;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.ViewManagement;
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
        LastestVolumeTile.CreateTileUpdater().Update(new TileNotification(await TileHelper.GetTileXmlDocument("Working.xml")));
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
