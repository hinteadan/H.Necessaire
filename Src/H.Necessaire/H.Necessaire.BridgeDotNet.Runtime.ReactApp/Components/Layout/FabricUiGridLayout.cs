using Bridge;
using Bridge.React;
using System.Linq;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class FabricUiGridLayout : ComponentBase<FabricUiGridLayout.Props, FabricUiGridLayout.State>
    {
        public FabricUiGridLayout(Props props, params Union<ReactElement, string>[] children) : base(props, children) { }
        public FabricUiGridLayout(params Union<ReactElement, string>[] children) : this(new Props { }, children) { }
        public FabricUiGridLayout(Props props, params ReactElement[] children) : this(props, children?.Select(x => (Union<ReactElement, string>)x).ToArray()) { }
        public FabricUiGridLayout(params ReactElement[] children) : this(new Props { }, children?.Select(x => (Union<ReactElement, string>)x).ToArray()) { }

        public override ReactElement Render()
        {
            ReactStyle reactStyle = new ReactStyle { Width = props.Width };
            if (props.HasGreedyHeight)
            {
                reactStyle = reactStyle.FlexNode(isVerticalFlow: true);
            }

            return
                DOM.Div(
                    new Attributes
                    {
                        ClassName = "ms-Grid",
                        Dir = Bridge.Html5.TextDirection.Ltr,
                        Style = reactStyle,
                    },

                    Children

                );
        }

        public class State : ComponentStateBase { }
        public class Props : ComponentPropsBase
        {
            public Union<string, int> Width { get; set; } = "100%";
            public bool HasGreedyHeight { get; set; } = false;
        }
    }
}
