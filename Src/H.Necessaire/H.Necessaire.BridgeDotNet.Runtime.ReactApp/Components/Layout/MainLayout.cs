using Bridge;
using Bridge.React;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class MainLayout : ComponentBase<MainLayout.Props, MainLayout.State>
    {
        #region Construct
        public MainLayout(Props props, params Union<ReactElement, string>[] children) : base(props, children) { }
        public MainLayout(params Union<ReactElement, string>[] children) : base(null, children) { }
        #endregion

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
                            FlexDirection = Bridge.Html5.FlexDirection.Column,
                        }
                        .FlexNode(),
                    },

                    DOM.Div(
                        new Attributes
                        {
                            Style = new ReactStyle().FlexNode(),
                        },
                        Children
                    ),

                    new BrandingFooter()

                );
        }

        public class State : ComponentStateBase { }

        public class Props : ComponentPropsBase { }
    }
}
