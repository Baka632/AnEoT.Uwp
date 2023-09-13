using System.Collections.ObjectModel;
using AnEoT.Uwp.Contracts;
using AnEoT.Uwp.Services;
using Windows.ApplicationModel;
using Windows.Storage;

namespace AnEoT.Uwp.ViewModels;

/// <summary>
/// <see cref="VolumeList"/> 的视图模型
/// </summary>
public class VolumeListViewModel : NotificationObject
{
    private readonly IVolumeProvider volumeProvider;
    private bool _IsLoading = true;
    private IEnumerable<VolumeListItem> _VolumeList;

    public bool IsLoading
    {
        get => _IsLoading;
        set
        {
            _IsLoading = value;
            OnPropertiesChanged();
        }
    }

    public IEnumerable<VolumeListItem> VolumeList
    {
        get => _VolumeList;
        set
        {
            _VolumeList = value;
            OnPropertiesChanged();
        }
    }

    public ObservableCollection<BreadcrumbBarItemInfo> BreadcrumbBarSource { get; } = new();
    public DelegateCommand NavigateToVolumePageCommand { get; }

    public VolumeListViewModel()
    {
        StorageFolder assetsFolder = Package.Current.InstalledLocation.GetFolderAsync("Assets").AsTask().Result;
        StorageFolder testFolder = assetsFolder.GetFolderAsync("Test").AsTask().Result;
        StorageFolder postsFolder = testFolder.GetFolderAsync("posts").AsTask().Result;

        volumeProvider = new FileVolumeProvider(postsFolder.Path);

        NavigateToVolumePageCommand = new DelegateCommand(obj =>
        {
            if (obj is string arg)
            {
                NavigationHelper.Navigate(typeof(VolumePage), arg);
            }
        });
    }

    /// <summary>
    /// 准备页面内容
    /// </summary>
    public async Task PreparePage()
    {
        IsLoading = true;
        IEnumerable<VolumeListItem> volList = await volumeProvider.GetVolumeListAsync();

        VolumeList = volList;

        BreadcrumbBarSource.Add(new BreadcrumbBarItemInfo("主页", () =>
        {
            NavigationHelper.Navigate(typeof(Views.MainFrame.MainFrame), null);
        }));
        BreadcrumbBarSource.Add(new BreadcrumbBarItemInfo("期刊列表", () =>
        {
            NavigationHelper.Navigate(typeof(VolumeList), null);
        }));

        IsLoading = false;
    }
}
