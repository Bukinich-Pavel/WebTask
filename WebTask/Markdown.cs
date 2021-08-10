using Markdig;
using Markdig.Parsers;
using Microsoft.AspNetCore.Html;
using Westwind.AspNetCore.Markdown;
// https://github.com/RickStrahl/Westwind.AspNetCore.Markdown
// https://www.codemag.com/article/1811071/Marking-up-the-Web-with-ASP.NET-Core-and-Markdown

namespace WebTask
{
    public static class Markdown
    {
        public static string Parse(string markdown,
                        bool usePragmaLines = false,
                        bool forceReload = false)
        {
            if (string.IsNullOrEmpty(markdown))
                return "";

            var parser = MarkdownParserFactory.GetParser(usePragmaLines, forceReload);
            return parser.Parse(markdown);
        }

        public static HtmlString ParseHtmlString(string markdown,
                        bool usePragmaLines = false,
                        bool forceReload = false)
        {
            return new HtmlString(Parse(markdown, usePragmaLines, forceReload));
        }
    }

    public static class MarkdownParserFactory
    {
        public static string DefaultMarkdownParserName { get; } = "MarkDig";

        /// <summary>
        /// Use a cached instance of the Markdown Parser to keep alive
        /// </summary>
        public static IMarkdownParser CurrentParser;

        /// <summary>
        /// Retrieves a cached instance of the markdown parser
        /// </summary>                
        /// <param name="forceLoad">Forces the parser to be reloaded - otherwise previously loaded instance is used</param>
        /// <param name="usePragmaLines">If true adds pragma line ids into the document that the editor can sync to</param>
        /// <param name="parserAddinId">optional addin id that checks for a registered Markdown parser</param>
        /// <returns>Mardown Parser Interface</returns>
        public static IMarkdownParser GetParser(bool usePragmaLines = false,
                                                bool forceLoad = false, string parserAddinId = null)
        {
            if (!forceLoad && CurrentParser != null)
                return CurrentParser;

            CurrentParser = new MarkdownParserMarkdig(usePragmaLines: usePragmaLines, force: forceLoad);

            return CurrentParser;
        }


    }
}
