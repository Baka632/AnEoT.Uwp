using System.Collections.ObjectModel;
using AnEoT.Uwp.Contracts;
using AnEoT.Uwp.Helpers.CustomMarkdown;
using AnEoT.Uwp.Models.Navigation;
using AnEoT.Uwp.Services;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.System;
using Windows.UI;

namespace AnEoT.Uwp.ViewModels;

/// <summary>
/// <seealso cref="ReadPage"/> 的视图模型
/// </summary>
public sealed class ReadPageViewModel : NotificationObject
{
    private readonly StorageFolder assetsFolder;
    private readonly IVolumeProvider volumeProvider;
    private readonly IArticleProvider articleProvider;
    private readonly IResourceProvider resourceProvider;
    private ArticleDetail _ArticleDetail;
    private VolumeInfo _VolumeInfo;
    private bool _IsLoading;
    internal readonly LauncherOptions DefaultLauncherOptionsForExternal = new()
    {
        TreatAsUntrusted = true
    };

    public bool IsLoading
    {
        get => _IsLoading;
        set
        {
            _IsLoading = value;
            OnPropertiesChanged();
        }
    }

    public ArticleDetail ArticleDetail
    {
        get => _ArticleDetail;
        set
        {
            _ArticleDetail = value;
            OnPropertiesChanged();
        }
    }
    
    public VolumeInfo VolumeInfo
    {
        get => _VolumeInfo;
        set
        {
            _VolumeInfo = value;
            OnPropertiesChanged();
        }
    }

    #region InfoBar Props & Fields

    private bool _InfoBarOpen;
    private string _InfoBarTitle;
    private string _InfoBarMessage;
    private InfoBarSeverity _InfoBarSeverity;

    public bool InfoBarOpen
    {
        get => _InfoBarOpen;
        set
        {
            _InfoBarOpen = value;
            OnPropertiesChanged();
        }
    }

    public string InfoBarTitle
    {
        get => _InfoBarTitle;
        set
        {
            _InfoBarTitle = value;
            OnPropertiesChanged();
        }
    }

    public string InfoBarMessage
    {
        get => _InfoBarMessage;
        set
        {
            _InfoBarMessage = value;
            OnPropertiesChanged();
        }
    }

    public InfoBarSeverity InfoBarSeverity
    {
        get => _InfoBarSeverity;
        set
        {
            _InfoBarSeverity = value;
            OnPropertiesChanged();
        }
    }
    #endregion

    public ObservableCollection<BreadcrumbBarItemInfo> BreadcrumbBarSource { get; } = new ();

    public ReadPageViewModel()
    {
        assetsFolder = Package.Current.InstalledLocation.GetFolderAsync("Assets").AsTask().Result;
        StorageFolder testFolder = assetsFolder.GetFolderAsync("Test").AsTask().Result;
        StorageFolder postsFolder = testFolder.GetFolderAsync("posts").AsTask().Result;

        volumeProvider = new FileVolumeProvider(postsFolder.Path);
        articleProvider = new FileArticleProvider(postsFolder.Path);
        resourceProvider = new FileResourceProvider();
    }

    /// <summary>
    /// 准备页面内容
    /// </summary>
    /// <param name="articleNavigationInfo">准备过程使用的数据源</param>
    public async Task PreparePage(ArticleNavigationInfo articleNavigationInfo)
    {
        IsLoading = true;

        try
        {
            VolumeInfo volumeInfo = await volumeProvider.GetVolumeInfoAsync(articleNavigationInfo.RawVolumeName);

            ArticleDetail = await articleProvider.GetArticleAsync(articleNavigationInfo.RawVolumeName,
                                                                  articleNavigationInfo.ArticleRawName);
            VolumeInfo = volumeInfo;

            BreadcrumbBarSource.Add(new BreadcrumbBarItemInfo("主页", () =>
            {
                NavigationHelper.Navigate(typeof(Views.MainFrame.MainFrame), null);
            }));
            BreadcrumbBarSource.Add(new BreadcrumbBarItemInfo("期刊列表", () =>
            {
                NavigationHelper.Navigate(typeof(VolumeList), null);
            }));
            BreadcrumbBarSource.Add(new BreadcrumbBarItemInfo(volumeInfo.Name, () =>
            {
                NavigationHelper.Navigate(typeof(VolumePage), volumeInfo.RawName);
            }));
            BreadcrumbBarSource.Add(new BreadcrumbBarItemInfo(ArticleDetail.Title, () =>
            {
                NavigationHelper.Navigate(typeof(ReadPage), articleNavigationInfo.ArticleRawName);
            }));
        }
        catch (DirectoryNotFoundException)
        {
            SetInfoBar("指定的文章无效", "请检查文章所属期刊是否正确", true, InfoBarSeverity.Error);
        }
        catch (ArgumentException)
        {
            SetInfoBar("指定的文章无效", "请检查文章名称是否正确", true, InfoBarSeverity.Error);
        }
    }

    public async Task LoadWebView(WebView sender, Color textColor)
    {
        if (ArticleDetail.MarkdownContent is not null)
        {
            VolumeInfo volumeInfo = VolumeInfo;

            Uri baseUri = new(resourceProvider.BaseUri.Replace("ms-appx", "ms-appx-web"), UriKind.Absolute);
            Uri uri = new(baseUri, volumeInfo.RawName);
            CustomMarkdownParser parser = new(false, false, uri.ToString());
            string content = parser.Parse(ArticleDetail.MarkdownContent);

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
            ((WebView)sender).ScriptNotify -= SetLoadingToFalseAndRemoveEventListening;
        }
    }

    public void SetInfoBar(string title, string message, bool isOpen, InfoBarSeverity severity)
    {
        InfoBarTitle = title;
        InfoBarMessage = message;
        InfoBarSeverity = severity;
        InfoBarOpen = isOpen;
    }
}
