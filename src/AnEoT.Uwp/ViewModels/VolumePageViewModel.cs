using System.Collections.ObjectModel;
using AnEoT.Uwp.Contracts;
using AnEoT.Uwp.Services;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace AnEoT.Uwp.ViewModels;

/// <summary>
/// <see cref="VolumePage"/> 的视图模型
/// </summary>
public sealed class VolumePageViewModel : NotificationObject
{
    private readonly StorageFolder assetsFolder;
    private readonly IVolumeProvider volumeProvider;
    private VolumeInfo _VolumeInfo;
    private BitmapImage _VolumeCover;

    public VolumePageViewModel()
    {
        assetsFolder = Package.Current.InstalledLocation.GetFolderAsync("Assets").AsTask().Result;
        StorageFolder testFolder = assetsFolder.GetFolderAsync("Test").AsTask().Result;
        StorageFolder postsFolder = testFolder.GetFolderAsync("posts").AsTask().Result;

        volumeProvider = new FileVolumeProvider(postsFolder.Path);
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

    public BitmapImage VolumeCover
    {
        get => _VolumeCover;
        set
        {
            _VolumeCover = value;
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

    /// <summary>
    /// 准备页面内容
    /// </summary>
    /// <param name="volumeRawName">目标原始期刊名称</param>
    public async Task PreparePage(string volumeRawName)
    {
        try
        {
            VolumeInfo volumeInfo = await volumeProvider.GetVolumeInfoAsync(volumeRawName);

            BreadcrumbBarSource.Add("主页");
            BreadcrumbBarSource.Add("期刊列表");
            BreadcrumbBarSource.Add(volumeInfo.Name);
            VolumeInfo = volumeInfo;
            Uri uri = await FileHelper.GetVolumeCover(volumeInfo.RawName);
            BitmapImage volCover = new(uri);
            VolumeCover = volCover;
        }
        catch (DirectoryNotFoundException)
        {
            SetInfoBar("指定的期刊无效", "请检查输入的期刊名称是否正确", true, InfoBarSeverity.Error);
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
