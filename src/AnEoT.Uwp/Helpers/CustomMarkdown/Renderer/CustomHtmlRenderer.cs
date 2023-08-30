using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;

namespace AnEoT.Uwp.Helpers.CustomMarkdown.Renderer
{
    /// <summary>
    /// 自定义的HTML渲染器
    /// </summary>
    public class CustomHtmlRenderer : HtmlRenderer
    {
        /// <summary>
        /// 使用指定的参数构造<seealso cref="CustomHtmlRenderer"/>的新实例
        /// </summary>
        public CustomHtmlRenderer(TextWriter writer, string baseUri = null) : base(writer)
        {
            {
                IMarkdownObjectRenderer linkInlineRenderer = ObjectRenderers.First(obj => obj is LinkInlineRenderer);

                int linkInlineRendererIndex = ObjectRenderers.IndexOf(linkInlineRenderer);
                ObjectRenderers.Insert(linkInlineRendererIndex, new CustomLinkInlineRenderer(baseUri));
                ObjectRenderers.Remove(linkInlineRenderer);
            }
        }
    }
}
