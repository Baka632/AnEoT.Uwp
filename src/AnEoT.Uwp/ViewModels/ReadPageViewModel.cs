using System.Collections.ObjectModel;
using AnEoT.Uwp.Contracts;
using AnEoT.Uwp.Models.Navigation;
using AnEoT.Uwp.Services;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel;
using Windows.Storage;

namespace AnEoT.Uwp.ViewModels;

/// <summary>
/// <seealso cref="ReadPage"/> 的视图模型
/// </summary>
public sealed class ReadPageViewModel : NotificationObject
{
    private readonly StorageFolder assetsFolder;
    private readonly IVolumeProvider volumeProvider;
    private readonly IArticleProvider articleProvider;
    private ArticleDetail _ArticleDetail;
    private VolumeInfo _VolumeInfo;

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

    public ObservableCollection<string> BreadcrumbBarSource { get; } = new ObservableCollection<string>();

    public ReadPageViewModel()
    {
        assetsFolder = Package.Current.InstalledLocation.GetFolderAsync("Assets").AsTask().Result;
        StorageFolder testFolder = assetsFolder.GetFolderAsync("Test").AsTask().Result;
        StorageFolder postsFolder = testFolder.GetFolderAsync("posts").AsTask().Result;

        volumeProvider = new FileVolumeProvider(postsFolder.Path);
        articleProvider = new FileArticleProvider(postsFolder.Path);
    }

    /// <summary>
    /// 准备页面内容
    /// </summary>
    /// <param name="articleNavigationInfo">准备过程使用的数据源</param>
    public async Task PreparePage(ArticleNavigationInfo articleNavigationInfo)
    {
        try
        {
            VolumeInfo volumeInfo = await volumeProvider.GetVolumeInfoAsync(articleNavigationInfo.RawVolumeName);

            ArticleDetail = await articleProvider.GetArticleAsync(articleNavigationInfo.RawVolumeName,
                                                                  articleNavigationInfo.ArticleRawName);
            VolumeInfo = volumeInfo;

            BreadcrumbBarSource.Add("主页");
            BreadcrumbBarSource.Add("期刊列表");
            BreadcrumbBarSource.Add(volumeInfo.Name);
            BreadcrumbBarSource.Add(ArticleDetail.Title);
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

    public void SetInfoBar(string title, string message, bool isOpen, InfoBarSeverity severity)
    {
        InfoBarTitle = title;
        InfoBarMessage = message;
        InfoBarSeverity = severity;
        InfoBarOpen = isOpen;
    }
}
