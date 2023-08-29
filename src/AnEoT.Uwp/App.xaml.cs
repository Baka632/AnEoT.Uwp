using System.Net;
using AnEoT.Uwp.Models.Navigation;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.System.Profile;

namespace AnEoT.Uwp;

/// <summary>
/// 提供特定于应用程序的行为，以补充默认的应用程序类。
/// </summary>
sealed partial class App : Application
{
    /// <summary>
    /// 初始化单实例应用程序对象。这是执行的创作代码的第一行，
    /// 已执行，逻辑上等同于 main() 或 WinMain()。
    /// </summary>
    public App()
    {
        this.InitializeComponent();
        this.Suspending += OnSuspending;
    }

    /// <summary>
    /// 获取应用程序名
    /// </summary>
    public static string AppDisplayName => ReswHelper.GetReswString("AppDisplayName");

    /// <summary>
    /// 在应用程序由最终用户正常启动时进行调用。
    /// 将在启动应用程序以打开特定文件等情况下使用。
    /// </summary>
    /// <param name="e">启动请求和过程的详细信息</param>
    protected override void OnLaunched(LaunchActivatedEventArgs e)
    {
        // 不要在窗口已包含内容时重复应用程序初始化，只需确保窗口处于活动状态
        if (Window.Current.Content is not Frame rootFrame)
        {
            // 创建要充当导航上下文的框架，并导航到第一页
            rootFrame = new Frame();

            rootFrame.NavigationFailed += OnNavigationFailed;

            if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                //TODO: 从之前挂起的应用程序加载状态
            }

            // 将框架放在当前窗口中
            Window.Current.Content = rootFrame;

            LoadMuxcResources();
        }

        if (e.PrelaunchActivated == false)
        {
            if (rootFrame.Content == null)
            {
                // 当导航堆栈尚未还原时，导航到第一页，并通过将所需信息作为导航参数传入来配置参数
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
            // 确保当前窗口处于活动状态
            Window.Current.Activate();
        }
    }

    protected override void OnActivated(IActivatedEventArgs args)
    {
        if (args.Kind == ActivationKind.Protocol)
        {
            ProtocolActivatedEventArgs eventArgs = args as ProtocolActivatedEventArgs;
            
            if (eventArgs != null && eventArgs.Uri is not null)
            {
                Uri uri = eventArgs.Uri;
                if (uri.Host.Equals("read", StringComparison.OrdinalIgnoreCase))
                {
                    if (uri.Segments.Length == 3)
                    {
                        //Article read
                        string rawVolumeInfo = uri.Segments[1].Trim('/');
                        string articleTitle = uri.Segments[2].Trim('/').TrimEnd(".html".ToCharArray());
                        ArticleNavigationInfo arg = new(rawVolumeInfo, articleTitle);
                        ArticleNavigation(arg);
                        return;
                    }
                    else if (uri.Segments.Length == 2)
                    {
                        string rawVolumeInfo = uri.Segments[1].Trim('/');
                        VolumeNavigation(rawVolumeInfo);
                        return;
                    }
                }
            }
        }

        //Fallback
        NavigateToMainPage();
    }

    private void ArticleNavigation(ArticleNavigationInfo arg)
    {
        if (Window.Current.Content is null)
        {
            LoadMuxcResources();
            Frame rootFrame = new();
            Window.Current.Content = rootFrame;

            rootFrame.Navigate(typeof(MainPage), arg);

            Window.Current.Activate();
        }
        else
        {
            NavigationHelper.Navigate(typeof(ReadPage), arg);
        }
    }
    
    private void VolumeNavigation(string arg)
    {
        if (Window.Current.Content is null)
        {
            LoadMuxcResources();
            Frame rootFrame = new();
            Window.Current.Content = rootFrame;

            rootFrame.Navigate(typeof(MainPage), arg);

            Window.Current.Activate();
        }
        else
        {
            NavigationHelper.Navigate(typeof(VolumePage), arg);
        }
    }
    
    private void NavigateToMainPage()
    {
        if (Window.Current.Content is null)
        {
            LoadMuxcResources();
            Frame rootFrame = new();
            Window.Current.Content = rootFrame;

            rootFrame.Navigate(typeof(MainPage));

            Window.Current.Activate();
        }
    }

    private void LoadMuxcResources()
    {
        bool isMobile = AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile";

        XamlControlsResources muxcStyle = new()
        {
            ControlsResourcesVersion = isMobile ? ControlsResourcesVersion.Version1 : ControlsResourcesVersion.Version2
        };
        Resources.MergedDictionaries.Add(muxcStyle);
    }

    /// <summary>
    /// 导航到特定页失败时调用
    /// </summary>
    ///<param name="sender">导航失败的框架</param>
    ///<param name="e">导航失败的详细信息</param>
    private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new Exception($"Failed to load Page {e.SourcePageType.FullName}");
    }

    /// <summary>
    /// 在将要挂起应用程序执行时调用。  在不知道应用程序
    /// 无需知道应用程序会被终止还是会恢复，
    /// 并让内存内容保持不变。
    /// </summary>
    /// <param name="sender">挂起的请求的源。</param>
    /// <param name="e">有关挂起请求的详细信息。</param>
    private void OnSuspending(object sender, SuspendingEventArgs e)
    {
        SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
        //TODO: 保存应用程序状态并停止任何后台活动
        deferral.Complete();
    }
}
