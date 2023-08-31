// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

using System;
using System.ComponentModel;
using AnEoT.Uwp.Helpers.CustomMarkdown;
using AnEoT.Uwp.Models.Navigation;
using Microsoft.UI.Xaml.Controls;
using Windows.System;

namespace AnEoT.Uwp.Views;

/// <summary>
/// 可用于自身或导航至 Frame 内部的空白页。
/// </summary>
public sealed partial class ReadPage : Page, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    private bool _IsLoading;
    private readonly static LauncherOptions DefaultLauncherOptionsForExternal = new()
    {
        TreatAsUntrusted = true
    };

    public ReadPageViewModel ViewModel { get; } = new ReadPageViewModel();
    public bool IsLoading
    {
        get => _IsLoading;
        set
        {
            _IsLoading = value;
            OnPropertiesChanged();
        }
    }

    public ReadPage()
    {
        this.InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is ArticleNavigationInfo navigationInfo)
        {
            IsLoading = true;
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

            string html = 
                $"""
                <div>{content}</div>
                """;

            try
            {
                sender.ScriptNotify += SetLoadingToFalseAndRemoveEventListening;

                //添加主内容
                await sender.InvokeScriptAsync("eval", new[]
                {
                    $"document.getElementById('mainContent').insertAdjacentHTML('afterbegin', `{html}`);"
                });

                //设置文本颜色
                await sender.InvokeScriptAsync("eval", new[]
                {
                    $"document.getElementById('mainContent').style.color = 'rgb({textColor.R}, {textColor.G}, {textColor.B})'",
                });

                sender.NavigationStarting += async (webView, args) =>
                {
                    args.Cancel = true;

                    await Launcher.LaunchUriAsync(args.Uri, DefaultLauncherOptionsForExternal);
                };

                await sender.InvokeScriptAsync("notifyWebView", Array.Empty<string>());
            }
            catch (Exception ex)
            {
                //脚本出错了...
                System.Diagnostics.Debug.WriteLine("[ReadPage] Exception occured!");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                IsLoading = false;
            }
        }

        void SetLoadingToFalseAndRemoveEventListening(object sender, NotifyEventArgs e)
        {
            IsLoading = false;
            ContentWebView.ScriptNotify -= SetLoadingToFalseAndRemoveEventListening;
        }
    }

    private void OnBreadcrumbBarItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
    {
        if (args.Item is BreadcrumbBarItemInfo itemInfo)
        {
            itemInfo.ClickAction?.Invoke();
        }
    }

    /// <summary>
    /// 通知运行时属性已经发生更改
    /// </summary>
    /// <param name="propertyName">发生更改的属性名称,其填充是自动完成的</param>
    public void OnPropertiesChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async void OnContentWebViewUnviewableContentIdentified(WebView sender, WebViewUnviewableContentIdentifiedEventArgs args)
    {
        await Launcher.LaunchUriAsync(args.Referrer, DefaultLauncherOptionsForExternal);
    }
}
