// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

using AnEoT.Uwp.ViewModels.MainFrame;

namespace AnEoT.Uwp.Views.MainFrame
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainFrame : Page
    {
        public MainFrameViewModel ViewModel { get; } = new MainFrameViewModel();

        public MainFrame()
        {
            this.InitializeComponent();

            MainReadPageFrame.Navigate(typeof(MainReadPage));
            MainSettingPageFrame.Navigate(typeof(MainSettingsPage));
            MainDownloadPageFrame.Navigate(typeof(MainDownloadPage));

            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
    }
}
