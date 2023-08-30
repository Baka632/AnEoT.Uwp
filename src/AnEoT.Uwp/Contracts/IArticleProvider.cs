namespace AnEoT.Uwp.Contracts;

/// <summary>
/// 定义一套与获取文章信息相关的方法
/// </summary>
public interface IArticleProvider
{
    /// <summary>
    /// 获取指定的文章
    /// </summary>
    /// <param name="volume">文章所属期刊的原始名称</param>
    /// <param name="article">文章原始名称</param>
    /// <returns>表示指定文章的 <see cref="ArticleDetail"/></returns>
    Task<ArticleDetail> GetArticleAsync(string volume, string article);

    /// <summary>
    /// 获取指定文章的信息
    /// </summary>
    /// <param name="volume">文章所属期刊的原始名称</param>
    /// <param name="article">文章原始名称</param>
    /// <returns>指定文章的信息</returns>
    Task<ArticleInfo> GetArticleInfoAsync(string volume, string article);
}
