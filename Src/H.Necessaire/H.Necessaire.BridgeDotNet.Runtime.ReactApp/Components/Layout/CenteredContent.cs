using Bridge;
using Bridge.React;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class CenteredContent : ComponentBase<CenteredContent.Props, CenteredContent.State>
    {
        public CenteredContent(Props props, params Union<ReactElement, string>[] children) : base(props, children) { }

        public CenteredContent(params Union<ReactElement, string>[] children) : this(new Props { }, children) { }

        public override ReactElement Render()
        {
            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            Flex = "1",
                            Display = Bridge.Html5.Display.Flex,
                            FlexDirection = Bridge.Html5.FlexDirection.Column,
                            JustifyContent = props.VerticalAlign,
                            MinHeight = props.MinHeight,
                        },
                    },
                    DOM.Div(
                        new Attributes
                        {
                            Style = new ReactStyle
                            {
                                Display = Bridge.Html5.Display.Flex,
                                JustifyContent = props.HorizontalAlign,
                            },
                        },

                        Children

                    )
                );
        }

        public class State : ComponentStateBase { }

        public class Props : ComponentPropsBase
        {
            public Union<string, int> MinHeight { get; set; } = 0;
            public Bridge.Html5.JustifyContent VerticalAlign { get; set; } = Bridge.Html5.JustifyContent.Center;
            public Bridge.Html5.JustifyContent HorizontalAlign { get; set; } = Bridge.Html5.JustifyContent.Center;
        }
    }
}
