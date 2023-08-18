using System.Threading.Tasks;

namespace AnEoT.Uwp.Contracts;

/// <summary>
/// 定义一套与获取文章信息相关的方法
/// </summary>
public interface IArticleProvider
{
    /// <summary>
    /// 获取指定的文章
    /// </summary>
    /// <param name="volume">文章所属期刊</param>
    /// <param name="title">文章标题</param>
    /// <returns>表示指定文章的 <see cref="ArticleDetail"/></returns>
    Task<ArticleDetail> GetArticleAsync(string volume, string title);

    /// <summary>
    /// 获取指定文章的信息
    /// </summary>
    /// <param name="volume">文章所属期刊</param>
    /// <param name="title">文章标题</param>
    /// <returns>指定文章的信息</returns>
    Task<ArticleInfo> GetArticleInfoAsync(string volume, string title);
}
