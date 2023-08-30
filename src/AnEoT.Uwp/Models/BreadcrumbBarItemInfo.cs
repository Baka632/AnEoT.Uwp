namespace AnEoT.Uwp.Models;

public readonly struct BreadcrumbBarItemInfo
{
    public string DisplayName { get; }
    public Action ClickAction { get; }

    public BreadcrumbBarItemInfo(string displayName, Action clickAction)
    {
        DisplayName = displayName;
        ClickAction = clickAction;
    }
}
