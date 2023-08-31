// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

using System.ComponentModel;
using Microsoft.UI.Xaml.Controls;

namespace AnEoT.Uwp.Views;

/// <summary>
/// 可用于自身或导航至 Frame 内部的空白页。
/// </summary>
public sealed partial class VolumePage : Page, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    private bool _IsLoading;

    public VolumePageViewModel ViewModel { get; }

    public bool IsLoading
    {
        get => _IsLoading;
        set
        {
            _IsLoading = value;
            OnPropertiesChanged();
        }
    }

    public VolumePage()
    {
        this.InitializeComponent();
        ViewModel = new VolumePageViewModel(ContentTextBlock.Inlines);
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is string volumeRawName)
        {
            IsLoading = true;
            await ViewModel.PreparePage(volumeRawName);
            IsLoading = false;
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
}
