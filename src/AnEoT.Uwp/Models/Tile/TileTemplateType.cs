namespace AnEoT.Uwp.Models.Tile;

/// <summary>
/// 指示磁贴类型的枚举
/// </summary>
public enum TileTemplateType
{
    /// <summary>
    /// 自适应磁贴，这是最推荐的类型，支持所有磁贴大小
    /// </summary>
    Adaptive,
    /// <summary>
    /// 图标磁贴，仅可用于小型与中型磁贴
    /// </summary>
    Iconic,
    /// <summary>
    /// Contract 磁贴，仅用于手机。支持小型、中型和宽型磁贴
    /// </summary>
    Contract,
    /// <summary>
    /// 联系人磁贴，在版本 1511 之后支持中型、宽型与大型磁贴。以前的版本仅支持中型磁贴与宽型磁贴，并只能用于手机
    /// </summary>
    People,
    /// <summary>
    /// 图像幻灯片磁贴，支持所有磁贴大小
    /// </summary>
    Photos
}
