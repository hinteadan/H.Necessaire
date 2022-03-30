using Bridge;
using Bridge.React;
using H.Necessaire.BridgeDotNet.Runtime.ReactApp.Core;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp.Components.Elements
{
    public class MarkdownView : ComponentBase<MarkdownView.Props, MarkdownView.State>
    {
        public MarkdownView(Props props, params Union<ReactElement, string>[] children) : base(props, children) { }

        public override ReactElement Render()
        {
            return
                DOM.Div(
                    new Attributes
                    {
                        Style = props?.Style,
                        ClassName = "markdown-view",
                        DangerouslySetInnerHTML = new RawHtml
                        {
                            Html = PrintMarkdown(),
                        }
                    }
                );
        }

        private string PrintMarkdown()
        {
            if (string.IsNullOrWhiteSpace(props?.MarkdownString))
                return string.Empty;

            return
                props?.IsInline == true
                ? MarkedJs.parseInline(props.MarkdownString)
                : MarkedJs.parse(props.MarkdownString, MarkedJsOptions.Default)
                ;
        }

        public class State : ComponentStateBase { }

        public class Props : ComponentPropsBase
        {
            public bool IsInline { get; set; } = false;
            public string MarkdownString { get; set; }
            public ReactStyle Style { get; set; } = null;
        }
    }

    public static class MarkdownViewExtensions
    {
        public static ReactElement PrintMarkdown(this string markdownString, bool isInline = false, ReactStyle style = null)
        {
            if (string.IsNullOrWhiteSpace(markdownString))
                return null;

            return
                new MarkdownView(
                    new MarkdownView.Props
                    {
                        MarkdownString = markdownString,
                        IsInline = isInline,
                        Style = style,
                    }
                );
        }

        public static ReactElement PrintMarkdownInline(this string markdownString)
        {
            return markdownString.PrintMarkdown(isInline: true, style: new ReactStyle { Display = Bridge.Html5.Display.Inline });
        }
    }
}
