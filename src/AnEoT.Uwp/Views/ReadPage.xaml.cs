// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

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
        SolidColorBrush colorBrush = (SolidColorBrush)Resources["SystemControlForegroundBaseHighBrush"];
        Windows.UI.Color textColor = colorBrush.Color;
        await ViewModel.LoadWebView(sender, textColor);
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
        await Launcher.LaunchUriAsync(args.Referrer, ViewModel.DefaultLauncherOptionsForExternal);
    }
}
