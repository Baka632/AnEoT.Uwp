namespace AnEoT.Uwp.Models.Markdown;

/// <summary>
/// 表示文章信息的结构
/// </summary>
public struct MarkdownArticleInfo : IEquatable<MarkdownArticleInfo>
{
    /// <summary>
    /// 构造一个已按默认值初始化的<see cref="ArticleInfo"/>的新实例
    /// </summary>
    public MarkdownArticleInfo()
    {
    }

    /// <summary>
    /// 文档标题
    /// </summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>
    /// 文档短标题
    /// </summary>
    public string ShortTitle { get; set; } = string.Empty;
    /// <summary>
    /// 文档类型图标
    /// </summary>
    public string Icon { get; set; } = string.Empty;
    /// <summary>
    /// 指示该项是否为文章的值
    /// </summary>
    public bool Article { get; set; } = true;
    /// <summary>
    /// 文章作者
    /// </summary>
    public string Author { get; set; } = string.Empty;
    /// <summary>
    /// 文档创建日期的字符串
    /// </summary>
    public string Date { get; set; } = string.Empty;
    /// <summary>
    /// 文档类别
    /// </summary>
    public IEnumerable<string> Category { get; set; }
    /// <summary>
    /// 文档标签
    /// </summary>
    public IEnumerable<string> Tag { get; set; }
    /// <summary>
    /// 文档在本期期刊的顺序
    /// </summary>
    public int Order { get; set; }
    /// <summary>
    /// 
    /// </summary>
    //我也不知道这有什么用
    public IDictionary<string, int> Dir { get; set; }

    /// <summary>
    /// <!--???-->
    /// </summary>
    public bool Star { get; set; }

    /// <summary>
    /// <!--???-->
    /// </summary>
    public bool Index { get; set; }

    /// <summary>
    /// 页面描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    public readonly override bool Equals(object obj)
    {
        return obj is MarkdownArticleInfo info && Equals(info);
    }

    public readonly bool Equals(MarkdownArticleInfo other)
    {
        bool isCategoryEqual = Category is not null && other.Category is not null
            ? Category.SequenceEqual(other.Category)
            : object.ReferenceEquals(Category, other.Category);
        
        bool isTagEqual = Tag is not null && other.Tag is not null
            ? Tag.SequenceEqual(other.Tag)
            : object.ReferenceEquals(Tag, other.Tag);
        
        bool isDirEqual = Dir is not null && other.Dir is not null
            ? Dir.SequenceEqual(other.Dir)
            : object.ReferenceEquals(Dir, other.Dir);

        return Title == other.Title &&
               ShortTitle == other.ShortTitle &&
               Icon == other.Icon &&
               Article == other.Article &&
               Author == other.Author &&
               Date == other.Date &&
               isCategoryEqual &&
               isTagEqual &&
               Order == other.Order &&
               isDirEqual &&
               Star == other.Star &&
               Index == other.Index &&
               Description == other.Description;
    }

    public override int GetHashCode()
    {
        int hashCode = -797563498;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Title);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ShortTitle);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Icon);
        hashCode = hashCode * -1521134295 + Article.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Author ?? string.Empty);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Date ?? string.Empty);
        hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<string>>.Default.GetHashCode(Category ?? Array.Empty<string>());
        hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<string>>.Default.GetHashCode(Tag ?? Array.Empty<string>());
        hashCode = hashCode * -1521134295 + Order.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<IDictionary<string, int>>.Default.GetHashCode(Dir);
        hashCode = hashCode * -1521134295 + Star.GetHashCode();
        hashCode = hashCode * -1521134295 + Index.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Description);
        return hashCode;
    }

    public static bool operator ==(MarkdownArticleInfo left, MarkdownArticleInfo right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(MarkdownArticleInfo left, MarkdownArticleInfo right)
    {
        return !(left == right);
    }

    public static implicit operator ArticleInfo(MarkdownArticleInfo info)
    {
        if (DateTimeOffset.TryParse(info.Date, out DateTimeOffset date) != true)
        {
            date = new DateTimeOffset();
        }

        string author = string.IsNullOrWhiteSpace(info.Author) ? "Another end of Terra" : info.Author;
        ArticleInfo articleInfo = new(
            info.Title, author, info.Description, date, info.Category, info.Tag, info.Order, info.ShortTitle);

        return articleInfo;
    }
}
