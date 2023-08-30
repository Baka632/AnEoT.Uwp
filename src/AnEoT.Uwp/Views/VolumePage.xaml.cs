// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

using Microsoft.UI.Xaml.Controls;

namespace AnEoT.Uwp.Views;

/// <summary>
/// 可用于自身或导航至 Frame 内部的空白页。
/// </summary>
public sealed partial class VolumePage : Page
{
    public VolumePageViewModel ViewModel { get; }
    internal readonly VolumePage Self;

    public VolumePage()
    {
        this.InitializeComponent();
        Self = this;
        ViewModel = new VolumePageViewModel(ContentTextBlock.Inlines);
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is string volumeRawName)
        {
            await ViewModel.PreparePage(volumeRawName);
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
