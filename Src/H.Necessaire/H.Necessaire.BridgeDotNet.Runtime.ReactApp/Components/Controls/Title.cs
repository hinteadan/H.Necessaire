using Bridge;
using Bridge.Html5;
using Bridge.React;
using System;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class Title : ComponentBase<Title.Props, Title.State>
    {
        public Title(Props props, params Union<ReactElement, string>[] children) : base(props, children) { }
        public Title(params Union<ReactElement, string>[] children) : this(new Props { }, children) { }

        public override ReactElement Render()
        {
            ReactStyle reactStyle = new ReactStyle
            {
                Color = Branding.HighlightTextColor.ToCssRGBA(),
                FontSize = Branding.Typography.FontSizeLarger.EmsCss,
                TextAlign = props.TextAlign,
                JustifyContent = JustifyContent.Center,
            };

            if (props.StyleDecorator != null)
                reactStyle = props.StyleDecorator(reactStyle);

            return
                DOM.H1(

                    new Attributes
                    {
                        Style = reactStyle,
                    },

                    Children

                );
        }

        public class State : ComponentStateBase { }
        public class Props : ComponentPropsBase
        {
            public TextAlign TextAlign { get; set; } = TextAlign.Center;
            public Func<ReactStyle, ReactStyle> StyleDecorator { get; set; }
        }
    }
}
