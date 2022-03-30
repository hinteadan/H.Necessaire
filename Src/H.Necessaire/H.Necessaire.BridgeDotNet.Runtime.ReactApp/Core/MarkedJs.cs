using Bridge;
using System;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp.Core
{
    /// <summary>
    /// https://marked.js.org/
    /// </summary>
    [External]
    [Name("marked")]
    public class MarkedJs
    {
        [External] public static extern string parse(string markdownString, MarkedJsOptions options = null, Action onParseCompleted = null);
        [External] public static extern string parseInline(string markdownString, MarkedJsOptions options = null, Action onParseCompleted = null);
        [External] public static extern void setOptions(MarkedJsOptions options);
        [External] public static extern void use(params MarkedJsExtension[] extensions);
        [External] public static extern object[] lexer(string markdown, object options);
        [External] public static extern string parser(object[] tokens, object options);
    }

    [External]
    [Name("marked.Renderer")]
    public class MarkedJsRenderer
    {

    }

    [External]
    [Name("marked.Tokenizer")]
    public class MarkedJsTokenizer
    {

    }

    [External]
    [Name("marked.Lexer")]
    public class MarkedJsLexer
    {
        public extern MarkedJsLexer(object options);

        [External] public extern object[] lex(string markdown);
    }

    /// <summary>
    /// https://marked.js.org/using_advanced#options
    /// </summary>
    public class MarkedJsOptions
    {
        public static MarkedJsOptions Default { get; } = new MarkedJsOptions();

        public string baseUrl { get; set; } = null;
        public bool breaks { get; set; } = false;
        public bool gfm { get; set; } = true;
        public bool headerIds { get; set; } = true;
        public string headerPrefix { get; set; } = string.Empty;
        /// <summary>
        /// Func<code, lang, callback(err, result)>
        /// 
        /// https://marked.js.org/using_advanced#highlight
        /// </summary>
        public Func<string, string, Action<object, string>, string> highlight { get; set; } = (text, lang, _) =>
        {
            if (HighlightJs.getLanguage(lang) != null)
                return HighlightJs.highlight(text, new HljsHighlightOptions { language = lang }).value;

            return HighlightJs.highlightAuto(text).value;
        };
        public string langPrefix { get; set; } = "language-";
        public bool mangle { get; set; } = true;
        public bool pedantic { get; set; } = false;
        /// <summary>
        /// https://marked.js.org/using_pro#renderer
        /// </summary>
        public object renderer { get; set; } = new MarkedJsRenderer();
        [Obsolete] public bool sanitize { get; set; } = false;
        /// <summary>
        /// Func<dirtyHtml, options> returns sanitized string
        /// </summary>
        [Obsolete] public Func<string, object, string> sanitizer { get; set; } = null;

        public bool silent { get; set; } = false;
        public bool smartLists { get; set; } = false;
        public bool smartypants { get; set; } = false;

        /// <summary>
        /// https://marked.js.org/using_pro#tokenizer
        /// </summary>
        public object tokenizer { get; set; } = new MarkedJsTokenizer();

        /// <summary>
        /// https://marked.js.org/using_pro#walk-tokens
        /// </summary>
        public Action<object> walkTokens { get; set; } = null;
        public bool xhtml { get; set; } = false;
    }

    /// <summary>
    /// https://marked.js.org/using_pro#extensions
    /// </summary>
    public class MarkedJsExtension
    {
        public string name { get; set; }
        /// <summary>
        /// Must be equal to 'block' or 'inline'. 
        /// </summary>
        public string level { get; set; }
        public Func<string, int> start { get; set; }
        public Func<string, object[], object> tokenizer { get; set; }
        public Func<object, string> renderer { get; set; }
        public string[] childTokens { get; set; } = null;
    }
}
