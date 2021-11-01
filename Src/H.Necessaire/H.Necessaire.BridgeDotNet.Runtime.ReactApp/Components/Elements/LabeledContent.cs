using Bridge;
using Bridge.React;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class LabeledContent : ComponentBase<LabeledContent.Props, LabeledContent.State>
    {
        public LabeledContent(Props props, params Union<ReactElement, string>[] children) : base(props, children) { }
        public LabeledContent(Union<ReactElement, string> label, params Union<ReactElement, string>[] children) : base(new Props { Label = label }, children) { }

        public override ReactElement Render()
        {
            return
                DOM.Div(new Attributes { Style = new ReactStyle { MarginTop = Branding.SizingUnitInPixels / 2, MarginBottom = Branding.SizingUnitInPixels / 2, }.FlexNode() },
                    DOM.Div(
                        new Attributes { Style = new ReactStyle { AlignContent = Bridge.Html5.AlignContent.Center }.FlexNode(isVerticalFlow: true) },
                        props.Label
                    ),
                    DOM.Div(
                        new Attributes { Style = new ReactStyle { Flex = "4", AlignContent = Bridge.Html5.AlignContent.Center }.FlexNode(isVerticalFlow: true) },
                        Children
                    )
                );
        }

        public class Props : ComponentPropsBase
        {
            public Union<ReactElement, string> Label { get; set; }
        }
        public class State : ComponentStateBase { }
    }
}
