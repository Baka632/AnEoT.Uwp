namespace AnEoT.Uwp.Models.Navigation;

/// <summary>
/// 为导航到特定文章页提供数据模型
/// </summary>
public readonly struct ArticleNavigationInfo
{
    /// <summary>
    /// 形如“2023-05”的原始期刊名称
    /// </summary>
    public string RawVolumeName { get; }

    /// <summary>
    /// 文章标题
    /// </summary>
    public string ArticleTitle { get; }

    /// <summary>
    /// 使用指定的参数构造 <see cref="ArticleNavigationInfo"/> 的新实例
    /// </summary>
    /// <param name="rawVolumeInfo">形如“2023-05”的原始期刊名称</param>
    /// <param name="articleTitle">文章标题</param>
    public ArticleNavigationInfo(string rawVolumeInfo, string articleTitle)
    {
        RawVolumeName = rawVolumeInfo;
        ArticleTitle = articleTitle;
    }
}
