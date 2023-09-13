using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace AnEoT.Uwp.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class VolumeList : Page
    {
        public VolumeListViewModel ViewModel { get; } = new();

        public VolumeList()
        {
            this.InitializeComponent();
        }

        private void OnBreadcrumbBarItemClicked(Microsoft.UI.Xaml.Controls.BreadcrumbBar sender, Microsoft.UI.Xaml.Controls.BreadcrumbBarItemClickedEventArgs args)
        {
            if (args.Item is BreadcrumbBarItemInfo itemInfo)
            {
                itemInfo.ClickAction?.Invoke();
            }
        }

        private async void OnPageLoading(FrameworkElement sender, object args)
        {
            await ViewModel.PreparePage();
        }
    }
}
