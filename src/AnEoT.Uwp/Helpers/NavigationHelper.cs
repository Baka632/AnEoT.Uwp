using Windows.UI.Core;
using Windows.UI.Xaml.Media.Animation;

namespace AnEoT.Uwp.Helpers;

public static class NavigationHelper
{
    public static Frame CurrentFrame { get; set; }
    public static SystemNavigationManager NavigationManager { get; } = SystemNavigationManager.GetForCurrentView();

    static NavigationHelper()
    {
        NavigationManager.BackRequested += BackRequested;
    }

    private static void BackRequested(object sender, BackRequestedEventArgs e)
    {
        GoBack(e);
    }

    public static void GoBack(BackRequestedEventArgs e)
    {
        if (CurrentFrame.CanGoBack)
        {
            e.Handled = true;
            CurrentFrame.GoBack();
        }
    }

    public static void GoForward(BackRequestedEventArgs e)
    {
        if (CurrentFrame.CanGoForward)
        {
            e.Handled = true;
            CurrentFrame.GoForward();
        }
    }

    public static void Navigate(Type sourcePageType, object parameter, NavigationTransitionInfo transitionInfo = null)
    {
        CurrentFrame.Navigate(sourcePageType, parameter, transitionInfo);
    }
}
