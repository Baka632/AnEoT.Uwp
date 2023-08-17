namespace AnEoT.Uwp.ViewModels;

public sealed class MainPageViewModel : NotificationObject
{
    public TitleBarHelper TitleBarHelper { get; }

    public MainPageViewModel(Frame frame)
    {
        TitleBarHelper = new TitleBarHelper(frame);
        NavigationHelper.CurrentFrame = frame;
        NavigationHelper.Navigate(typeof(Views.MainFrame.MainFrame), null);
    }
}
