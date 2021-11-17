using Bridge;
using Bridge.React;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class CenteredCurtain : ComponentBase<CenteredCurtain.Props, CenteredCurtain.State>
    {
        public CenteredCurtain(Props props, params Union<ReactElement, string>[] children) : base(props, children) { }
        public CenteredCurtain(params Union<ReactElement, string>[] children) : this(new Props { }, children) { }

        public override ReactElement Render()
        {
            return
                new Curtain(new Curtain.Props { },
                    new CenteredContent(

                        new FormLayout(
                            new FormLayout.Props { LayoutMode = FormLayoutMode.OnePerRowSmall, RowSpacing = Branding.SizingUnitInPixels * 2 },

                            DOM.Div(
                                new Attributes
                                {
                                    Style = new ReactStyle { JustifyContent = Bridge.Html5.JustifyContent.Center, }.FlexNode()
                                },

                                Children
                            )
                        )
                    )
                );
        }

        public class State : ComponentStateBase { }
        public class Props : ComponentPropsBase { }
    }
}
