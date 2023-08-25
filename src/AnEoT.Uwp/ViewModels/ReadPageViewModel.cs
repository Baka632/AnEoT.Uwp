using System.Collections.ObjectModel;
using AnEoT.Uwp.Contracts;
using AnEoT.Uwp.Models.Navigation;
using AnEoT.Uwp.Services;
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

    public ArticleDetail ArticleDetail
    {
        get => _ArticleDetail;
        set
        {
            _ArticleDetail = value;
            OnPropertiesChanged();
        }
    }

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
        VolumeInfo volumeInfo = await volumeProvider.GetVolumeInfoAsync(articleNavigationInfo.RawVolumeName);

        BreadcrumbBarSource.Add("主页");
        BreadcrumbBarSource.Add("期刊列表");
        BreadcrumbBarSource.Add(volumeInfo.Name);
        BreadcrumbBarSource.Add(articleNavigationInfo.ArticleTitle);

        ArticleDetail = await articleProvider.GetArticleAsync(articleNavigationInfo.RawVolumeName,
                                                              articleNavigationInfo.ArticleTitle);
    }
}
