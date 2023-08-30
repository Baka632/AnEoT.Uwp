// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

using AnEoT.Uwp.Helpers.CustomMarkdown;
using AnEoT.Uwp.Models.Navigation;
using Microsoft.UI.Xaml.Controls;

namespace AnEoT.Uwp.Views;

/// <summary>
/// 可用于自身或导航至 Frame 内部的空白页。
/// </summary>
public sealed partial class ReadPage : Page
{
    public ReadPageViewModel ViewModel { get; } = new ReadPageViewModel();

    public ReadPage()
    {
        this.InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is ArticleNavigationInfo navigationInfo)
        {
            await ViewModel.PreparePage(navigationInfo);
            ContentWebView.Navigate(new Uri("ms-appx-web:///Assets/Web/Html/MarkdownPageTemplate.html"));
        }
    }

    private async void OnNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
    {
        if (ViewModel.ArticleDetail.MarkdownContent is not null)
        {
            SolidColorBrush colorBrush = (SolidColorBrush)Resources["SystemControlForegroundBaseHighBrush"];
            Windows.UI.Color textColor = colorBrush.Color;

            VolumeInfo volumeInfo = ViewModel.VolumeInfo;
            CustomMarkdownParser parser = new(false, false, $"ms-appx-web:///Assets/Test/posts/{volumeInfo.RawName}");
            string content = parser.Parse(ViewModel.ArticleDetail.MarkdownContent);

            string html = $"<div>{content}</div>";

            try
            {
                //添加主内容
                await sender.InvokeScriptAsync("eval", new[]
                {
                    $"document.getElementById('mainContent').innerHTML = `{html}`",
                });

                //设置文本颜色
                await sender.InvokeScriptAsync("eval", new[]
                {
                    $"document.getElementById('mainContent').style.color = 'rgb({textColor.R}, {textColor.G}, {textColor.B})'",
                });
            }
            catch (Exception ex)
            {
                //脚本出错了...
                System.Diagnostics.Debug.WriteLine("[ReadPage] Exception occured!");
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }

    private void OnBreadcrumbBarItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
    {
        if (args.Item is BreadcrumbBarItemInfo itemInfo)
        {
            itemInfo.ClickAction?.Invoke();
        }
    }
}
