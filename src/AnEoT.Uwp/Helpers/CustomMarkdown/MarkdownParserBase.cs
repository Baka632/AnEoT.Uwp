using System.Text.RegularExpressions;

namespace AnEoT.Uwp.Helpers.CustomMarkdown;

#region License

/*
 **************************************************************
 *  Author: Rick Strahl 
 *          (c) West Wind Technologies, 2008 - 2018
 *          http://www.west-wind.com/
 * 
 * Created: 09/08/2018
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 **************************************************************  
*/

#endregion

/// <summary>
/// Base class that includes various fix up methods for custom parsing
/// that can be called by the specific implementations.
/// </summary>
public abstract class MarkdownParserBase : IMarkdownParser
{
    protected static Regex strikeOutRegex =
        new("~~.*?~~", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

    /// <summary>
    /// Parses markdown
    /// </summary>
    /// <param name="markdown"></param>
    /// <param name="sanitizeHtml">If true sanitizes the generated HTML by removing script tags and other common XSS issues.
    /// Note: Not a complete XSS solution but addresses the most obvious vulnerabilities. For more thourough HTML sanitation
    /// </param>
    /// <returns></returns>
    public abstract string Parse(string markdown, bool sanitizeHtml = true);

    /// <summary>
    /// Parses strikeout text ~~text~~. Single line (to linebreak) allowed only.
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    protected string ParseStrikeout(string html)
    {
        if (html == null)
            return null;

        var matches = strikeOutRegex.Matches(html);
        foreach (Match match in matches)
        {
            string val = match.Value;

            if (match.Value.Contains('\n'))
                continue;

            val = "<del>" + val.Substring(2, val.Length - 4) + "</del>";
            html = html.Replace(match.Value, val);
        }

        return html;
    }

    static readonly Regex YamlExtractionRegex = new Regex("^---[\n,\r\n].*?^---[\n,\r\n]",
        RegexOptions.Singleline | RegexOptions.Multiline);

    /// <summary>
    /// Strips 
    /// </summary>
    /// <param name="markdown"></param>
    /// <returns></returns>
    public string StripFrontMatter(string markdown)
    {
        string extractedYaml = null;
        var match = YamlExtractionRegex.Match(markdown);
        if (match.Success)
            extractedYaml = match.Value;

        if (!string.IsNullOrEmpty(extractedYaml))
            markdown = markdown.Replace(extractedYaml, "");

        return markdown;
    }

    #region Html Sanitation

    /// <summary>
    /// Global list of tags that are cleaned up by the script sanitation
    /// as a pipe separated list.
    ///
    /// You can change this value which applies to the scriptScriptTags
    /// option, but it is an application wide global setting.
    /// 
    /// Default: script|iframe|object|embed|form
    /// </summary>
    public static string HtmlSanitizeTagBlackList { get; set; } = "script|iframe|object|embed|form";

    /// <summary>
    /// Parses out script tags that might not be encoded yet
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    protected string Sanitize(string html)
    {
        return StringUtils.SanitizeHtml(html,HtmlSanitizeTagBlackList);
    }

    #endregion

    public static Regex fontAwesomeIconRegEx = new Regex(@"@icon-.*?[\s|\.|\,|\<]");

    /// <summary>
    /// Post processing routine that post-processes the HTML and 
    /// replaces @icon- with fontawesome icons
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    protected string ParseFontAwesomeIcons(string html)
    {
        var matches = fontAwesomeIconRegEx.Matches(html);
        foreach (Match match in matches)
        {
            string iconblock = match.Value.Substring(0, match.Value.Length - 1);
            string icon = iconblock.Replace("@icon-", "");
            html = html.Replace(iconblock, "<i class=\"fa fa-" + icon + "\"></i> ");
        }

        return html;
    }

}
