namespace AnEoT.Uwp.Models;

/// <summary>
/// 为页面导航栏提供信息的结构
/// </summary>
public readonly struct BreadcrumbBarItemInfo
{
    /// <summary>
    /// 要在导航栏上显示的名称
    /// </summary>
    public string DisplayName { get; }
    /// <summary>
    /// 点击时要进行的操作
    /// </summary>
    public Action ClickAction { get; }

    /// <summary>
    /// 使用指定的参数构造 <see cref="BreadcrumbBarItemInfo"/> 的新实例
    /// </summary>
    /// <param name="displayName">显示名称</param>
    /// <param name="clickAction">点击时进行的操作</param>
    public BreadcrumbBarItemInfo(string displayName, Action clickAction)
    {
        DisplayName = displayName;
        ClickAction = clickAction;
    }
}
