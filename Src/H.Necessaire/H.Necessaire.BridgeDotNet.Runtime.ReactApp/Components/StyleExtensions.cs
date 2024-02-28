using Bridge;
using Bridge.Html5;
using Bridge.React;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public static class StyleExtensions
    {
        public static ReactStyle ScrollContent(this ReactStyle style)
        {
            if (style == null) return style;

            style.Overflow = Overflow.Auto;

            return style;
        }

        public static ReactStyle FlexNode(this ReactStyle style, bool isVerticalFlow = false, Union<string, int> size = null)
        {
            if (style == null) return style;

            if (size == null && style["flex"] == null)
                style.Flex = "1";
            else if (isVerticalFlow)
                style.Height = size;
            else
                style.Width = size;

            style.Display = Display.Flex;
            style.MinHeight = 0;
            if (isVerticalFlow)
                style.FlexDirection = FlexDirection.Column;

            return style;
        }
        public static ReactStyle FlexNodeH(this ReactStyle style, Union<string, int> size = null) => style.FlexNode(isVerticalFlow: false, size: size);
        public static ReactStyle FlexNodeV(this ReactStyle style, Union<string, int> size = null) => style.FlexNode(isVerticalFlow: true, size: size);
    }
}
