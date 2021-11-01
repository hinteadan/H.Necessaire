using Bridge;
using Bridge.React;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class PaddedContent : ComponentBase<PaddedContent.Props, PaddedContent.State>
    {
        public PaddedContent(Props props, params Union<ReactElement, string>[] children) : base(props, children) { }
        public PaddedContent(params Union<ReactElement, string>[] children) : this(new Props { }, children) { }

        public override ReactElement Render()
        {
            return
                DOM.Div
                (
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            PaddingLeft = Branding.SizingUnitInPixels,
                            PaddingRight = Branding.SizingUnitInPixels,
                        }.FlexNode(isVerticalFlow: props.IsVerticalFlow)
                    },

                    Children

                );
        }

        public class State : ComponentStateBase { }
        public class Props : ComponentPropsBase
        {
            public bool IsVerticalFlow { get; set; } = true;
        }
    }
}
