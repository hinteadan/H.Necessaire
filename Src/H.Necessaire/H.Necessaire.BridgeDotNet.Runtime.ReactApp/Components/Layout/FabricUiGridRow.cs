using Bridge;
using Bridge.React;
using System.Linq;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class FabricUiGridRow : ComponentBase<FabricUiGridRow.Props, FabricUiGridRow.State>
    {
        public FabricUiGridRow(Props props, params Union<ReactElement, string>[] children) : base(props, children) { }
        public FabricUiGridRow(params Union<ReactElement, string>[] children) : this(new Props { }, children) { }
        public FabricUiGridRow(Props props, params ReactElement[] children) : this(props, children.Select(x => (Union<ReactElement, string>)x).ToArray()) { }
        public FabricUiGridRow(params ReactElement[] children) : this(new Props { }, children.Select(x => (Union<ReactElement, string>)x).ToArray()) { }

        public override ReactElement Render()
        {
            ReactStyle reactStyle = new ReactStyle { MarginTop = props.RowSpacing };
            if (props.HasGreedyHeight)
            {
                reactStyle = reactStyle.FlexNode(isVerticalFlow: true);
            }

            return
                DOM.Div(
                    new Attributes
                    {
                        ClassName = "ms-Grid-row",
                        Style = reactStyle,
                    },

                    Children

                );
        }

        public class State : ComponentStateBase { }
        public class Props : ComponentPropsBase
        {
            public int RowSpacing { get; set; } = Branding.SizingUnitInPixels;
            public bool HasGreedyHeight { get; set; } = false;
        }
    }
}
