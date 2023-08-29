using AnEoT.Uwp.Models.Navigation;
using Windows.ApplicationModel.Core;
using Windows.System.Profile;
using Windows.UI.Core;

namespace AnEoT.Uwp.Views;

/// <summary>
/// 可用于自身或导航至 Frame 内部的空白页。
/// </summary>
public sealed partial class MainPage : Page
{
    public MainPageViewModel ViewModel { get; }
    private bool IsTitleBarTextBlockForwardBegun = false;
    private bool IsFirstRun = true;

    public MainPage()
    {
        this.InitializeComponent();

        ViewModel = new MainPageViewModel(ContentFrame);

        if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
        {
            TitleBarTextBlock.Visibility = Visibility.Collapsed;
        }
        else
        {
            TitleBarHelper.BackButtonVisibilityChangedEvent += OnBackButtonVisibilityChanged;
            TitleBarHelper.TitleBarVisibilityChangedEvent += OnTitleBarVisibilityChanged;
        }

        _ = AcrylicHelper.TrySetAcrylicBrush(this);
    }

    private void OnTitleBarVisibilityChanged(CoreApplicationViewTitleBar bar)
    {
        if (bar.IsVisible)
        {
            StartTitleBarAnimation(Visibility.Visible);
        }
        else
        {
            StartTitleBarAnimation(Visibility.Collapsed);
        }
    }

    private void OnBackButtonVisibilityChanged(BackButtonVisibilityChangedEventArgs args)
    {
        StartTitleTextBlockAnimation(args.BackButtonVisibility);
    }

    private void StartTitleTextBlockAnimation(AppViewBackButtonVisibility buttonVisibility)
    {
        switch (buttonVisibility)
        {
            case AppViewBackButtonVisibility.Disabled:
            case AppViewBackButtonVisibility.Visible:
                if (IsTitleBarTextBlockForwardBegun)
                {
                    goto default;
                }
                TitleBarTextBlockForward.Begin();
                IsTitleBarTextBlockForwardBegun = true;
                break;
            case AppViewBackButtonVisibility.Collapsed:
                TitleBarTextBlockBack.Begin();
                IsTitleBarTextBlockForwardBegun = false;
                break;
            default:
                break;
        }
    }

    private void StartTitleBarAnimation(Visibility visibility)
    {
        if (IsFirstRun)
        {
            IsFirstRun = false;
            TitleBar.Visibility = visibility;
            return;
        }

        switch (visibility)
        {
            case Visibility.Visible:
                TitleBarShow.Begin();
                break;
            case Visibility.Collapsed:
            default:
                break;
        }
        TitleBar.Visibility = visibility;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is ArticleNavigationInfo info)
        {
            NavigationHelper.Navigate(typeof(ReadPage), info);
        }
        else if (e.Parameter is string rawVolumeInfo && string.IsNullOrWhiteSpace(rawVolumeInfo) != true)
        {
            NavigationHelper.Navigate(typeof(VolumePage), rawVolumeInfo);
        }
    }
}
