using System.Text.RegularExpressions;
using System.Text;
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;

namespace AnEoT.Uwp.Helpers;

/// <summary>
/// 为 Markdown 处理提供通用操作
/// </summary>
public static class MarkdownHelper
{
    /// <summary>
    /// 经过配置的 Markdown 管道，可解析大多数 Markdown 语法
    /// </summary>
    public static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
            .UseEmphasisExtras(Markdig.Extensions.EmphasisExtras.EmphasisExtraOptions.Default)
            .UseAdvancedExtensions()
            .UseListExtras()
            .UseEmojiAndSmiley(true)
            .UseYamlFrontMatter()
            .Build();

    /// <summary>
    /// 获取由Markdown中Front Matter转换而来的模型
    /// </summary>
    /// <param name="markdown">Markdown文件内容</param>
    /// <typeparam name="T">模型类型</typeparam>
    /// <returns>转换得到的模型</returns>
    public static T GetFromFrontMatter<T>(string markdown)
    {
        MarkdownDocument doc = Markdown.Parse(markdown, Pipeline);
        YamlFrontMatterBlock yamlBlock = doc.Descendants<YamlFrontMatterBlock>().FirstOrDefault();

        if (yamlBlock is not null)
        {
            string yaml = markdown.Substring(yamlBlock.Span.Start, yamlBlock.Span.Length);
            T model = YamlHelper.ReadYaml<T>(yaml);

            return model;
        }
        else
        {
            throw new ArgumentException("无法通过指定的参数解析出模型，Markdown可能没有Front Matter信息");
        }
    }

    /// <summary>
    /// 尝试获取由 Markdown 中 Front Matter 转换而来的模型
    /// </summary>
    /// <param name="markdown">Markdown 文件内容</param>
    /// <param name="result">转换得到的模型</param>
    /// <typeparam name="T">模型类型</typeparam>
    /// <returns>指示操作是否成功的值</returns>
    public static bool TryGetFromFrontMatter<T>(string markdown, out T result)
    {
        MarkdownDocument doc = Markdown.Parse(markdown, Pipeline);
        YamlFrontMatterBlock yamlBlock = doc.Descendants<YamlFrontMatterBlock>().FirstOrDefault();

        if (yamlBlock is not null)
        {
            string yaml = markdown.Substring(yamlBlock.Span.Start, yamlBlock.Span.Length);
            if (YamlHelper.TryReadYaml(yaml, out T model) && model is not null)
            {
                result = model;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }
        else
        {
            result = default;
            return false;
        }
    }

    /// <summary>
    /// 获取 Markdown 的文章引言
    /// </summary>
    /// <param name="markdown">Markdown 文件内容</param>
    /// <returns>文章引言，若不存在，则返回空字符串</returns>
    public static string GetArticleQuote(string markdown)
    {
        if (markdown.Contains("<!-- more -->") != true)
        {
            return string.Empty;
        }

        string quote = null;
        MarkdownDocument doc = Markdown.Parse(markdown, Pipeline);
        YamlFrontMatterBlock yamlBlock = doc.Descendants<YamlFrontMatterBlock>().FirstOrDefault();

        foreach (MarkdownObject item in doc)
        {
            if (item is HtmlBlock htmlBlock)
            {
                string html = markdown.Substring(htmlBlock.Span.Start, htmlBlock.Span.Length);
                if (html == "<!-- more -->")
                {
                    if (yamlBlock is not null)
                    {
                        quote = markdown[(yamlBlock.Span.End + 1)..htmlBlock.Span.Start].Trim();
                    }
                    else
                    {
                        quote = markdown[..htmlBlock.Span.Start].Trim();
                    }
                    break;
                }
            }
        }

        return quote ?? string.Empty;
    }

    //Modified from https://learn.microsoft.com/en-us/answers/questions/594274/convert-html-text-to-plain-text-in-c
    /// <summary>
    /// 将 Markdown 文本转化为纯文本
    /// </summary>
    /// <param name="markdown">Markdown 文本</param>
    /// <returns>转化后的纯文本</returns>
    public static string ToPlainText(string markdown)
    {
        string html = Markdown.ToHtml(markdown, Pipeline);

        // Remove HEAD tag  
        html = Regex.Replace(html, "<head.*?</head>", ""
                            , RegexOptions.IgnoreCase | RegexOptions.Singleline);
        // Remove any JavaScript  
        html = Regex.Replace(html, "<script.*?</script>", ""
          , RegexOptions.IgnoreCase | RegexOptions.Singleline);
        // Replace special characters like &, <, >, " etc.  
        StringBuilder stringBuilder = new(html);
        // Note: There are many more special characters, these are just  
        // most common. You can add new characters in this arrays if needed  
        string[] OldWords = {"&nbsp;", "&amp;", "&quot;", "&lt;",
   "&gt;", "&reg;", "&copy;", "&bull;", "&trade;","&#39;"};
        string[] NewWords = { " ", "&", "\"", "<", ">", "Â®", "Â©", "â€¢", "â„¢", "\'" };
        for (int i = 0; i < OldWords.Length; i++)
        {
            stringBuilder.Replace(OldWords[i], NewWords[i]);
        }
        // Check if there are line breaks (<br>) or paragraph (<p>)  
        stringBuilder.Replace("<br>", "\n<br>");
        stringBuilder.Replace("<br ", "\n<br ");
        stringBuilder.Replace("<p ", "\n<p ");
        // Finally, remove all HTML tags and return plain text  
        return Regex.Replace(
          stringBuilder.ToString(), "<[^>]*>", "").Trim();
    }
}
