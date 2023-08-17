// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

using Windows.Data.Xml.Dom;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.ViewManagement;

namespace AnEoT.Uwp.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainFrame : Page
    {
        public MainFrameViewModel ViewModel { get; } = new MainFrameViewModel();

        private readonly UISettings uiSettings = new();

        public MainFrame()
        {
            this.InitializeComponent();

            uiSettings.ColorValuesChanged += async (sender, obj) =>
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, SetupTile);
            };
        }

        private void OnPageLoading(FrameworkElement sender, object args) => SetupTile();

        private async void SetupTile()
        {
            WelcomeTile.VisualElements.BackgroundColor = uiSettings.GetColorValue(UIColorType.Accent);
            XmlDocument content = new();
            content.LoadXml(await ViewModel.GetTileXml("Welcome.xml"));

            WelcomeTile.CreateTileUpdater().Update(new TileNotification(content));
        }
    }
}
