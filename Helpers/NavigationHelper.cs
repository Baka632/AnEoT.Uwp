using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace AnEoT.Uwp.Helpers
{
    public static class NavigationHelper
    {
        public static Frame CurrentFrame { get; set; }
        public static SystemNavigationManager NavigationManager { get; } = SystemNavigationManager.GetForCurrentView();

        static NavigationHelper()
        {
            NavigationManager.BackRequested += BackRequested;
        }

        private static void BackRequested(object sender, BackRequestedEventArgs e) => GoBack();

        public static void GoBack()
        {
            if (CurrentFrame.CanGoBack)
            {
                CurrentFrame.GoBack();
            }
        }

        public static void GoForward()
        {
            if (CurrentFrame.CanGoForward)
            {
                CurrentFrame.GoForward();
            }
        }

        public static void Navigate(Type sourcePageType, object parameter, NavigationTransitionInfo transitionInfo = null)
        {
            CurrentFrame.Navigate(sourcePageType, parameter, transitionInfo);
        }
    }
}
