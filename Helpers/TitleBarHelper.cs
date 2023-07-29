using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI;
using Windows.System.Profile;

namespace AnEoT.Uwp.Helpers
{
    public class TitleBarHelper
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
                CurrentFrame.Navigated += CurrentFrame_Navigated;
            }
        }

        private void OnTitleBarVisibilityChanged(CoreApplicationViewTitleBar sender, object args)
        {
            TitleBarVisibilityChangedEvent?.Invoke(sender);
        }

        private void CurrentFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            NavigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibilityToBoolean(CurrentFrame.CanGoBack);
            BackButtonVisibilityChangedEvent?.Invoke(new BackButtonVisibilityChangedEventArgs(NavigationManager.AppViewBackButtonVisibility));

            static AppViewBackButtonVisibility AppViewBackButtonVisibilityToBoolean(bool canGoBack)
            {
                if (canGoBack)
                {
                    return AppViewBackButtonVisibility.Visible;
                }
                else
                {
                    return AppViewBackButtonVisibility.Collapsed;
                }
            }
        }
    }

    public class BackButtonVisibilityChangedEventArgs : EventArgs
    {
        public AppViewBackButtonVisibility BackButtonVisibility { get; }

        public BackButtonVisibilityChangedEventArgs(AppViewBackButtonVisibility visibility)
        {
            BackButtonVisibility = visibility;
        }
    }
}
