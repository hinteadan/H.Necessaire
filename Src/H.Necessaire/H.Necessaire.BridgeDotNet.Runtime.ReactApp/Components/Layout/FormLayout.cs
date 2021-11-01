using Bridge;
using Bridge.React;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class FormLayout : ComponentBase<FormLayout.Props, FormLayout.State>
    {
        public FormLayout(Props props, params Union<ReactElement, string>[] children) : base(props, children) { }
        public FormLayout(params Union<ReactElement, string>[] children) : this(new Props { }, children) { }

        public override ReactElement Render()
        {
            return
                new FabricUiGridLayout
                (
                    RenderRows()
                );
        }

        private ReactElement[] RenderRows()
        {
            if (!Children?.Any() ?? true)
                return null;

            var batches = Children.Where(x => x != null).Batch(state.BatchSizeForLayout[props.LayoutMode]);

            return
                batches
                .Select(RenderRow)
                .ToArray()
                ;
        }

        private ReactElement RenderRow(IEnumerable<Union<ReactElement, string>> cellContents)
        {
            return
                new FabricUiGridRow(new FabricUiGridRow.Props { RowSpacing = props.RowSpacing }, cellContents.Select(RenderColumn).ToArray());
        }

        private ReactElement RenderColumn(Union<ReactElement, string> cellContent)
        {
            return
                new FabricUiGridColumn
                (
                    state.ColumnStyleForLayout[props.LayoutMode],

                    cellContent

                )
                ;
        }

        public class State : ComponentStateBase
        {
            public Dictionary<FormLayoutMode, FabricUiGridColumn.Props> ColumnStyleForLayout { get; } = new Dictionary<FormLayoutMode, FabricUiGridColumn.Props>
            {
                { FormLayoutMode.InlineAsScreenGrows, new FabricUiGridColumn.Props { Size = FabricUiGridColumnSize.X12, SizeLarge = FabricUiGridColumnSize.X6, SizeXXLarge = FabricUiGridColumnSize.X3 } },
                { FormLayoutMode.OnePerRowSmall, new FabricUiGridColumn.Props { Size = FabricUiGridColumnSize.X12, SizeXLarge = FabricUiGridColumnSize.X4, PushXLarge = FabricUiGridColumnSize.X4, SizeXXXLarge = FabricUiGridColumnSize.X2, PushXXXLarge = FabricUiGridColumnSize.X5 } },
                { FormLayoutMode.OnePerRow, new FabricUiGridColumn.Props { Size = FabricUiGridColumnSize.X12, SizeXLarge = FabricUiGridColumnSize.X6, PushXLarge = FabricUiGridColumnSize.X3, SizeXXXLarge = FabricUiGridColumnSize.X4, PushXXXLarge = FabricUiGridColumnSize.X4 } },
                { FormLayoutMode.OnePerRowLarge, new FabricUiGridColumn.Props { Size = FabricUiGridColumnSize.X12, SizeXLarge = FabricUiGridColumnSize.X10, PushXLarge = FabricUiGridColumnSize.X1, SizeXXXLarge = FabricUiGridColumnSize.X8, PushXXXLarge = FabricUiGridColumnSize.X2 } },
                { FormLayoutMode.OnePerRowFill, new FabricUiGridColumn.Props { Size = FabricUiGridColumnSize.X12 } },
            };

            public Dictionary<FormLayoutMode, int> BatchSizeForLayout { get; } = new Dictionary<FormLayoutMode, int>
            {
                { FormLayoutMode.InlineAsScreenGrows, 4 },
                { FormLayoutMode.OnePerRowSmall, 1 },
                { FormLayoutMode.OnePerRow, 1 },
                { FormLayoutMode.OnePerRowLarge, 1 },
                { FormLayoutMode.OnePerRowFill, 1 },
            };
        }

        public class Props : ComponentPropsBase
        {
            public FormLayoutMode LayoutMode { get; set; } = FormLayoutMode.InlineAsScreenGrows;
            public int RowSpacing { get; set; } = Branding.SizingUnitInPixels;
        }
    }

    public enum FormLayoutMode
    {
        InlineAsScreenGrows = 0,
        OnePerRowSmall = -1,
        OnePerRow = 1,
        OnePerRowLarge = 2,
        OnePerRowFill = 10,
    }
}
