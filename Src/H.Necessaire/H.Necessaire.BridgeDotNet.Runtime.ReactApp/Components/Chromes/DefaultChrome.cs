using Bridge;
using Bridge.React;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class DefaultChrome : ComponentBase<DefaultChrome.Props, DefaultChrome.State>
    {
        public DefaultChrome(Props props, params Union<ReactElement, string>[] children) : base(props, children) { }
        public DefaultChrome(params Union<ReactElement, string>[] children) : base(new Props(), children) { }

        public override ReactElement Render()
        {
            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            Width = "100%",
                            Height = "100%",
                            Display = Bridge.Html5.Display.Flex,
                            FlexDirection = Bridge.Html5.FlexDirection.Column,
                        }
                    },

                    new BrandingHeader(props?.HeaderProps, props?.Title),

                    new MainLayout(
                        new ScrollableContent(
                            Children
                        )
                    )
                );
        }

        public class State : ComponentStateBase { }

        public class Props : ComponentPropsBase
        {
            public Union<ReactElement, string> Title { get; set; } = Config.Get("PageTitlePrefix")?.ToString() ?? "H.Necessaire";

            public BrandingHeader.Props HeaderProps { get; set; } = new BrandingHeader.Props();
        }
    }
}
