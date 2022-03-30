using Bridge;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp.Core
{
    [External]
    [Name("hljs")]
    public class HighlightJs
    {
        [External] public static extern void highlightAll();

        [External] public static extern HljsHighlightResult highlight(string text, HljsHighlightOptions options);

        [External] public static extern HljsHighlightResult highlightAuto(string text);

        [External] public static extern void highlightElement(object element);

        [External] public static extern object getLanguage(string lang);
    }

    public class HljsHighlightOptions
    {
        public string language { get; set; }
    }

    public class HljsHighlightResult
    {
        public string value { get; set; }
    }
}
