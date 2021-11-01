using Bridge;
using Bridge.React;
using System.Linq;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class FlexFormLayout : ComponentBase<FlexFormLayout.Props, FlexFormLayout.State>
    {
        public FlexFormLayout(Props props, params Union<ReactElement, string>[] children) : base(props, children) { }
        public FlexFormLayout(params Union<ReactElement, string>[] children) : this(new Props { }, children) { }

        public override ReactElement Render()
        {
            return
                new FabricUiGridLayout
                (
                    new FabricUiGridLayout.Props { HasGreedyHeight = true },

                    RenderRows()
                );
        }

        private ReactElement[] RenderRows()
        {
            if (!Children?.Any() ?? true)
                return null;

            return
                Children
                .Select(RenderRow)
                .ToArray()
                ;
        }

        private ReactElement RenderRow(Union<ReactElement, string> cellContent)
        {
            return
                new FabricUiGridRow(new FabricUiGridRow.Props { HasGreedyHeight = true }, RenderColumn(cellContent));
        }

        private ReactElement RenderColumn(Union<ReactElement, string> cellContent)
        {
            return
                new FabricUiGridColumn
                (
                    new FabricUiGridColumn.Props
                    {
                        HasGreedyHeight = true,
                        Size = FabricUiGridColumnSize.X12,
                        SizeXLarge = FabricUiGridColumnSize.X6,
                        PushXLarge = FabricUiGridColumnSize.X3,
                        SizeXXXLarge = FabricUiGridColumnSize.X4,
                        PushXXXLarge = FabricUiGridColumnSize.X4
                    },

                    cellContent

                )
                ;
        }

        public class State : ComponentStateBase { }

        public class Props : ComponentPropsBase
        {

        }
    }
}
