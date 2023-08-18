namespace AnEoT.Uwp.Models;

/// <summary>
/// 表示一期期刊
/// </summary>
public readonly struct VolumeDetail : IEquatable<VolumeDetail>
{
    /// <summary>
    /// 期刊名称
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 期刊文章列表
    /// </summary>
    public IEnumerable<ArticleDetail> Articles { get; }

    /// <summary>
    /// 使用指定的参数构造 <see cref="VolumeDetail"/> 的新实例
    /// </summary>
    /// <param name="name">期刊名称</param>
    /// <param name="articles">期刊文章列表</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> 为 null 或空白</exception>
    public VolumeDetail(string name, IEnumerable<ArticleDetail> articles)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException($"“{nameof(name)}”不能为 null 或空。", nameof(name));
        }

        Name = name;
        Articles = articles;
    }

    public override bool Equals(object obj)
    {
        return obj is VolumeDetail detail && Equals(detail);
    }

    public bool Equals(VolumeDetail other)
    {
        bool isArticlesEqual = Articles is not null && other.Articles is not null
            ? Articles.SequenceEqual(other.Articles)
            : object.ReferenceEquals(Articles, other.Articles);

        return Name == other.Name && isArticlesEqual;
    }

    public override int GetHashCode()
    {
        int hashCode = -1779391520;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
        hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<ArticleDetail>>.Default.GetHashCode(Articles);
        return hashCode;
    }

    public static bool operator ==(VolumeDetail left, VolumeDetail right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(VolumeDetail left, VolumeDetail right)
    {
        return !(left == right);
    }
}
