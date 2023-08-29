using AnEoT.Uwp.Helpers.CustomMarkdown.Renderer;
using Markdig.Renderers;

namespace AnEoT.Uwp.Helpers.CustomMarkdown
{
    internal class CustomMarkdownParser : MarkdownParserMarkdig
    {
        private readonly string baseUri;

        /// <summary>
        /// 使用指定的参数构造<seealso cref="CustomMarkdownParser"/>的新实例
        /// </summary>
        public CustomMarkdownParser(bool usePragmaLines, bool forceLoad, string baseUri = null) : base(usePragmaLines, forceLoad)
        {
            this.baseUri = baseUri;
        }

        /// <inheritdoc/>
        public override string Parse(string markdown, bool sanitizeHtml = true)
        {
            if (string.IsNullOrEmpty(markdown))
            {
                return string.Empty;
            }

            string html;
            using (StringWriter stringWriter = new())
            {
                IMarkdownRenderer renderer = new CustomHtmlRenderer(stringWriter, baseUri);
                Markdig.Markdown.Convert(markdown, renderer, Pipeline);
                html = stringWriter.ToString();
            }

            html = ParseFontAwesomeIcons(html);
            if (sanitizeHtml)
            {
                html = Sanitize(html);
            }

            return html;
        }
    }
}
