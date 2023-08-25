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
    /// 形如“2023-05”的原始期刊名称
    /// </summary>
    public string RawName { get; }

    public IEnumerable<ArticleInfo> Articles { get; }

    /// <summary>
    /// 使用指定的参数构造 <see cref="VolumeInfo"/> 的新实例
    /// </summary>
    /// <param name="name">期刊名称</param>
    /// <param name="rawName">形如“2023-05”的原始期刊名称</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> 为 null 或空白</exception>
    public VolumeInfo(string name, string rawName, IEnumerable<ArticleInfo> articles)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException($"“{nameof(name)}”不能为 null 或空。", nameof(name));
        }

        Name = name;
        RawName = rawName;
        Articles = articles ?? throw new ArgumentNullException(nameof(articles));
    }

    public override bool Equals(object obj)
    {
        return obj is VolumeInfo info && Equals(info);
    }

    public bool Equals(VolumeInfo other)
    {
        bool isArticlesEqual = Articles is not null && other.Articles is not null
            ? Articles.SequenceEqual(other.Articles)
            : object.ReferenceEquals(Articles, other.Articles);

        return Name == other.Name && RawName == other.RawName && isArticlesEqual;
    }

    public override int GetHashCode()
    {
        int hashCode = 1593016734;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(RawName);
        hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<ArticleInfo>>.Default.GetHashCode(Articles);
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
