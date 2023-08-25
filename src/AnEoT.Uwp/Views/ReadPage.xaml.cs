// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

using AnEoT.Uwp.Models.Navigation;
using Markdig;

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

            string content = Markdown.ToHtml(ViewModel.ArticleDetail.MarkdownContent, MarkdownHelper.Pipeline);
            //HACK: 无效
            string html =
$"""
<head>
    <meta charset="utf-8" />
    <link href="ms-appx-web://Assets/Css/site.css" rel="stylesheet" type="text/css" />
    <link href="ms-appx-web://Assets/Css/index.css" rel="stylesheet" type="text/css" />
    <link href="ms-appx-web://Assets/Css/palette.css" rel="stylesheet" type="text/css" />
    <link href="https://unpkg.com/lxgw-wenkai-screen-webfont@1.6.0/style.css" rel="stylesheet" type="text/css" />
</head>
{content}
""";

            ContentWebView.NavigateToString(html);
        }
    }
}
