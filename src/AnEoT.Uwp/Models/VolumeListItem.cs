namespace AnEoT.Uwp.Models;

/// <summary>
/// 为期刊列表页提供相关信息的结构
/// </summary>
public readonly struct VolumeListItem : IEquatable<VolumeListItem>
{
    /// <summary>
    /// 期刊名称
    /// </summary>
    public string Name { get; } = string.Empty;

    /// <summary>
    /// 形如“2023-05”的原始期刊名称
    /// </summary>
    public string RawName { get; } = string.Empty;

    /// <summary>
    /// 期刊封面的链接
    /// </summary>
    public string CoverLink { get; } = string.Empty;

    /// <summary>
    /// 使用指定的参数构造 <see cref="VolumeListItem"/> 的新实例
    /// </summary>
    /// <param name="name">期刊名称</param>
    /// <param name="rawName">形如“2023-05”的原始期刊名称</param>
    /// <param name="coverLink">期刊封面链接</param>
    public VolumeListItem(string name, string rawName, string coverLink)
    {
        Name = name;
        RawName = rawName;
        CoverLink = coverLink;
    }

    public override bool Equals(object obj)
    {
        return obj is VolumeListItem item && Equals(item);
    }

    public bool Equals(VolumeListItem other)
    {
        return Name == other.Name &&
               RawName == other.RawName &&
               CoverLink == other.CoverLink;
    }

    public override int GetHashCode()
    {
        int hashCode = 301511696;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(RawName);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CoverLink);
        return hashCode;
    }

    public static bool operator ==(VolumeListItem left, VolumeListItem right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(VolumeListItem left, VolumeListItem right)
    {
        return !(left == right);
    }
}
