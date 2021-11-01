using Bridge;
using Bridge.React;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class ScrollableContent : ComponentBase<ScrollableContent.Props, ScrollableContent.State>
    {
        public ScrollableContent(params Union<ReactElement, string>[] children) : base(new Props { }, children) { }

        public override ReactElement Render()
        {
            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle().FlexNode().ScrollContent(),
                    },
                    Children
                );
        }

        public class State : ComponentStateBase { }

        public class Props : ComponentPropsBase { }
    }
}
