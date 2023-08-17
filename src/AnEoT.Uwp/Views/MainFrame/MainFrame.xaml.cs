// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

using Windows.Data.Xml.Dom;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.ViewManagement;

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
        }
    }
}
