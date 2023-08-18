namespace AnEoT.Uwp.Models;

/// <summary>
/// 提供期刊相关信息的结构
/// </summary>
public readonly struct VolumeInfo : IEquatable<VolumeInfo>
{
    /// <summary>
    /// 期刊名称
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 使用指定的参数构造 <see cref="VolumeInfo"/> 的新实例
    /// </summary>
    /// <param name="name">期刊名称</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> 为 null 或空白</exception>
    public VolumeInfo(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException($"“{nameof(name)}”不能为 null 或空。", nameof(name));
        }

        Name = name;
    }

    public override bool Equals(object obj)
    {
        return obj is VolumeInfo info && Equals(info);
    }

    public bool Equals(VolumeInfo other)
    {
        return Name == other.Name;
    }

    public override int GetHashCode()
    {
        int hashCode = -288255790;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
        return hashCode;
    }

    public static bool operator ==(VolumeInfo left, VolumeInfo right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(VolumeInfo left, VolumeInfo right)
    {
        return !(left == right);
    }
}
