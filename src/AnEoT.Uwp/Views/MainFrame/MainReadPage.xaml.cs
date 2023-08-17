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
    private readonly UISettings uiSettings = new();

    public MainReadPage()
    {
        this.InitializeComponent();

        uiSettings.ColorValuesChanged += async (sender, obj) =>
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, SetupTile);
        };
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        SetupTile();
    }

    private async void SetupTile()
    {
        WelcomeTile.VisualElements.BackgroundColor = uiSettings.GetColorValue(UIColorType.Accent);
        XmlDocument content = new();
        content.LoadXml(await TileHelper.GetTileXml("Welcome.xml"));

        WelcomeTile.CreateTileUpdater().Update(new TileNotification(content));
    }
}
