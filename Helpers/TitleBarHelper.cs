using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.System.Profile;

namespace AnEoT.Uwp.Helpers;

public sealed class TitleBarHelper
{
    public CoreApplicationViewTitleBar CoreTitleBar { get; } = CoreApplication.GetCurrentView().TitleBar;
    public ApplicationViewTitleBar PresentationTitleBar { get; } = ApplicationView.GetForCurrentView().TitleBar;
    public SystemNavigationManager NavigationManager { get; } = SystemNavigationManager.GetForCurrentView();
    public Frame CurrentFrame { get; }

    public static event Action<BackButtonVisibilityChangedEventArgs> BackButtonVisibilityChangedEvent;
    public static event Action<CoreApplicationViewTitleBar> TitleBarVisibilityChangedEvent;

    public TitleBarHelper(Frame frame)
    {
        if (AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile")
        {
            CoreTitleBar.ExtendViewIntoTitleBar = true;
            CoreTitleBar.IsVisibleChanged += OnTitleBarVisibilityChanged;

            #region TitleBarColor
            PresentationTitleBar.ButtonBackgroundColor = Colors.Transparent;
            Color ForegroundColor = Application.Current.RequestedTheme switch
            {
                ApplicationTheme.Light => Colors.Black,
                ApplicationTheme.Dark => Colors.White,
                _ => Colors.White,
            };
            PresentationTitleBar.ButtonForegroundColor = ForegroundColor;
            #endregion

            CurrentFrame = frame;
            CurrentFrame.Navigated += OnCurrentFrameNavigated;
        }
    }

    private void OnTitleBarVisibilityChanged(CoreApplicationViewTitleBar sender, object args)
    {
        TitleBarVisibilityChangedEvent?.Invoke(sender);
    }

    private void OnCurrentFrameNavigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        NavigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibilityToBoolean(CurrentFrame.CanGoBack);
        BackButtonVisibilityChangedEvent?.Invoke(new BackButtonVisibilityChangedEventArgs(NavigationManager.AppViewBackButtonVisibility));

        static AppViewBackButtonVisibility AppViewBackButtonVisibilityToBoolean(bool canGoBack)
        {
            return canGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }
    }
}

public sealed class BackButtonVisibilityChangedEventArgs : EventArgs
{
    public AppViewBackButtonVisibility BackButtonVisibility { get; }

    public BackButtonVisibilityChangedEventArgs(AppViewBackButtonVisibility visibility)
    {
        BackButtonVisibility = visibility;
    }
}
