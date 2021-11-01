using Bridge;
using Bridge.React;
using System;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class Curtain : ComponentBase<Curtain.Props, Curtain.State>
    {
        public Curtain(Props props, params Union<ReactElement, string>[] children) : base(props, children) { }
        public Curtain(Props props) : this(props, null) { }
        public Curtain() : this(new Props { }, null) { }

        public override ReactElement Render()
        {
            ReactStyle reactStyle = new ReactStyle
            {
                Position = Bridge.Html5.Position.Fixed,
                Width = "100vw",
                Height = "100vh",
                Top = 0,
                Left = 0,
                BackgroundColor = Branding.BackgroundColorTranslucent.ToCssRGBA(),
                Display = Bridge.Html5.Display.Flex,
                ZIndex = int.MaxValue.ToString(),
            };

            if (props.StyleDecorator != null)
                reactStyle = props.StyleDecorator(reactStyle);

            return
                DOM.Div
                (
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
            public Func<ReactStyle, ReactStyle> StyleDecorator { get; set; }
        }
    }
}
