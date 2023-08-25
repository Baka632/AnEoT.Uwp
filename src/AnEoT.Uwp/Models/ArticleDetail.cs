namespace AnEoT.Uwp.Models;

/// <summary>
/// 表示一篇文章
/// </summary>
public readonly struct ArticleDetail : IEquatable<ArticleDetail>
{
    /// <summary>
    /// 文章标题
    /// </summary>
    public string Title { get; } = string.Empty;

    /// <summary>
    /// 文档短标题
    /// </summary>
    public string ShortTitle { get; } = string.Empty;

    /// <summary>
    /// 文章作者
    /// </summary>
    public string Author { get; } = string.Empty; 

    /// <summary>
    /// 文章描述
    /// </summary>
    public string Description { get; } = string.Empty;

    /// <summary>
    /// 文章日期
    /// </summary>
    public DateTimeOffset Date { get; }

    /// <summary>
    /// 文章的 Markdown 内容
    /// </summary>
    public string MarkdownContent { get; } = string.Empty;

    /// <summary>
    /// 文章类别
    /// </summary>
    public IEnumerable<string> Category { get; }

    /// <summary>
    /// 文章标签
    /// </summary>
    public IEnumerable<string> Tag { get; }

    /// <summary>
    /// 文档在本期期刊的顺序，可为负数
    /// </summary>
    public int? Order { get; }

    /// <summary>
    /// 使用指定的参数构造 <see cref="ArticleDetail"/> 的新实例
    /// </summary>
    /// <param name="title">文章标题</param>
    /// <param name="author">文章作者</param>
    /// <param name="description">文章描述</param>
    /// <param name="date">文章日期</param>
    /// <param name="markdownContent">文章的 Markdown 内容</param>
    /// <param name="category">文章类别</param>
    /// <param name="tag">文章标签</param>
    /// <param name="order">文章顺序，可以为负数</param>
    /// <param name="shortTitle">文章短标题</param>
    public ArticleDetail(string title, string author, string description, DateTimeOffset date, string markdownContent, IEnumerable<string> category, IEnumerable<string> tag, int? order = null, string shortTitle = "")
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException($"“{nameof(title)}”不能为 null 或空。", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(author))
        {
            throw new ArgumentException($"“{nameof(author)}”不能为 null 或空。", nameof(author));
        }

        Title = title;
        Author = author;
        Description = description ?? throw new ArgumentException($"“{nameof(description)}”不能为 null。", nameof(description));
        Date = date;
        MarkdownContent = markdownContent;
        Category = category;
        Tag = tag;

        //可选属性
        Order = order;
        ShortTitle = shortTitle;
    }

    public override bool Equals(object obj)
    {
        return obj is ArticleDetail detail && Equals(detail);
    }

    public bool Equals(ArticleDetail other)
    {
        bool isCategoryEqual = Category is not null && other.Category is not null
            ? Category.SequenceEqual(other.Category)
            : object.ReferenceEquals(Category, other.Category);

        bool isTagEqual = Tag is not null && other.Tag is not null
            ? Tag.SequenceEqual(other.Tag)
            : object.ReferenceEquals(Tag, other.Tag);

        return Title == other.Title &&
               Author == other.Author &&
               Description == other.Description &&
               Date.Equals(other.Date) &&
               Order == other.Order &&
               ShortTitle == other.ShortTitle &&
               MarkdownContent == other.MarkdownContent &&
               isCategoryEqual &&
               isTagEqual;
    }

    public override int GetHashCode()
    {
        int hashCode = -1417262674;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Title);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ShortTitle);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Author);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Description);
        hashCode = hashCode * -1521134295 + Date.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MarkdownContent);
        hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<string>>.Default.GetHashCode(Category);
        hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<string>>.Default.GetHashCode(Tag);
        hashCode = hashCode * -1521134295 + Order.GetHashCode();
        return hashCode;
    }

    public static bool operator ==(ArticleDetail left, ArticleDetail right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ArticleDetail left, ArticleDetail right)
    {
        return !(left == right);
    }
}
