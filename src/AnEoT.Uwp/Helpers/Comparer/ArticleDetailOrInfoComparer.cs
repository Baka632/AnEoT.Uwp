namespace AnEoT.Uwp.Helpers.Comparer;

/// <summary>
/// 为 <see cref="ArticleDetail"/> 或 <see cref="ArticleInfo"/> 提供比较方法，以用于排序
/// </summary>
public readonly struct ArticleDetailOrInfoComparer : IComparer<ArticleInfo>, IComparer<ArticleDetail>
{
    /// <inheritdoc/>
    public int Compare(ArticleInfo x, ArticleInfo y)
    {
        if (object.ReferenceEquals(x, y) || object.ReferenceEquals(x.Order, y.Order))
        {
            return 0;
        }

        return CompareCore(x.Order, y.Order);
    }

    /// <inheritdoc/>
    public int Compare(ArticleDetail x, ArticleDetail y)
    {
        if (object.ReferenceEquals(x, y) || object.ReferenceEquals(x.Order, y.Order))
        {
            return 0;
        }

        return CompareCore(x.Order, y.Order);
    }

    private int CompareCore(int? x, int? y)
    {
        if (x is null)
        {
            return -1;
        }
        else if (y is null)
        {
            return 1;
        }

        //前面的判断语句已经排除了Order属性为null的情况
        //所以下面的代码直接使用Nullable<int>（int?）的Value属性
        if (x > 0 && y > 0)
        {
            return Comparer<int>.Default.Compare(x.Value, y.Value);
        }
        else if (x > 0 && y < 0)
        {
            return -1;
        }
        else if (x < 0 && y > 0)
        {
            return 1;
        }
        else
        {
            return Comparer<int>.Default.Compare(-x.Value, -y.Value);
        }
    }
}
