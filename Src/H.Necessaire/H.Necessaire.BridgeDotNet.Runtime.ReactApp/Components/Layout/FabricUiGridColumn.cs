using Bridge;
using Bridge.React;


namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    /*
     
        Breakpoint	Range
        small	320px - 479px
        medium	480px - 639px
        large	640px - 1023px
        extra large	1024px - 1365px
        exta extra large	1366px - 1919px
        extra extra extra large	1920px and up
         
         */

    public class FabricUiGridColumn : ComponentBase<FabricUiGridColumn.Props, FabricUiGridColumn.State>
    {
        private const string SizeSmall = "sm";
        private const string SizeMedium = "md";
        private const string SizeLarge = "lg";
        private const string SizeXLarge = "xl";
        private const string SizeXXLarge = "xxl";
        private const string SizeXXXLarge = "xxxl";

        public FabricUiGridColumn(Props props, params Union<ReactElement, string>[] children) : base(props, children) { }
        public FabricUiGridColumn(params Union<ReactElement, string>[] children) : this(new Props { }, children) { }

        public override ReactElement Render()
        {
            ReactStyle reactStyle = new ReactStyle { };
            if (props.HasGreedyHeight)
            {
                reactStyle = reactStyle.FlexNode(isVerticalFlow: true);
            }

            return
                DOM.Div(
                    new Attributes
                    {
                        ClassName = $"ms-Grid-col{SizeToClassNames()}{PushToClassNames()}{PullToClassNames()}{HiddensToClassNames()}",
                        Style = reactStyle,
                    },

                    Children

                );
        }

        private string HiddensToClassNames()
        {
            string result = string.Empty;

            if (props.HiddenSmall)
                result += $" ms-hiddenSm";

            if (props.HiddenMedium)
                result += $" ms-hiddenMd";
            if (props.HiddenMediumAndLess)
                result += $" ms-hiddenMdDown";
            if (props.HiddenMediumAndMore)
                result += $" ms-hiddenMdUp";

            if (props.HiddenLarge)
                result += $" ms-hiddenLg";
            if (props.HiddenLargeAndLess)
                result += $" ms-hiddenLgDown";
            if (props.HiddenLargeAndMore)
                result += $" ms-hiddenLgUp";

            if (props.HiddenXLarge)
                result += $" ms-hiddenXl";
            if (props.HiddenXLargeAndLess)
                result += $" ms-hiddenXlDown";
            if (props.HiddenXLargeAndMore)
                result += $" ms-hiddenXlUp";

            if (props.HiddenXXLarge)
                result += $" ms-hiddenXxl";
            if (props.HiddenXXLargeAndLess)
                result += $" ms-hiddenXxlDown";
            if (props.HiddenXXLargeAndMore)
                result += $" ms-hiddenXxlUp";

            if (props.HiddenXXXLarge)
                result += $" ms-hiddenXxxl";

            return result;
        }

        private string PushToClassNames()
        {
            string result = string.Empty;

            if (props.Push != null)
                result += $" ms-{SizeSmall}Push{(int)props.Push.Value}";
            if (props.PushMedium != null)
                result += $" ms-{SizeMedium}Push{(int)props.PushMedium.Value}";
            if (props.PushLarge != null)
                result += $" ms-{SizeLarge}Push{(int)props.PushLarge.Value}";
            if (props.PushXLarge != null)
                result += $" ms-{SizeXLarge}Push{(int)props.PushXLarge.Value}";
            if (props.PushXXLarge != null)
                result += $" ms-{SizeXXLarge}Push{(int)props.PushXXLarge.Value}";
            if (props.PushXXXLarge != null)
                result += $" ms-{SizeXXXLarge}Push{(int)props.PushXXXLarge.Value}";

            return result;
        }

        private string PullToClassNames()
        {
            string result = string.Empty;

            if (props.Pull != null)
                result += $" ms-{SizeSmall}Push{(int)props.Pull.Value}";
            if (props.PullMedium != null)
                result += $" ms-{SizeMedium}Pull{(int)props.PullMedium.Value}";
            if (props.PullLarge != null)
                result += $" ms-{SizeLarge}Pull{(int)props.PullLarge.Value}";
            if (props.PullXLarge != null)
                result += $" ms-{SizeXLarge}Pull{(int)props.PullXLarge.Value}";
            if (props.PullXXLarge != null)
                result += $" ms-{SizeXXLarge}Pull{(int)props.PullXXLarge.Value}";
            if (props.PullXXXLarge != null)
                result += $" ms-{SizeXXXLarge}Pull{(int)props.PullXXXLarge.Value}";

            return result;
        }

        private string SizeToClassNames()
        {
            string result = $" ms-{SizeSmall}{(int)props.Size}";

            if (props.SizeMedium != null)
                result += $" ms-{SizeMedium}{(int)props.SizeMedium.Value}";
            if (props.SizeLarge != null)
                result += $" ms-{SizeLarge}{(int)props.SizeLarge.Value}";
            if (props.SizeXLarge != null)
                result += $" ms-{SizeXLarge}{(int)props.SizeXLarge.Value}";
            if (props.SizeXXLarge != null)
                result += $" ms-{SizeXXLarge}{(int)props.SizeXXLarge.Value}";
            if (props.SizeXXXLarge != null)
                result += $" ms-{SizeXXXLarge}{(int)props.SizeXXXLarge.Value}";

            return result;
        }

        public class State : ComponentStateBase { }
        public class Props : ComponentPropsBase
        {
            public FabricUiGridColumnSize Size { get; set; } = FabricUiGridColumnSize.X1;
            public FabricUiGridColumnSize? SizeMedium { get; set; }
            public FabricUiGridColumnSize? SizeLarge { get; set; }
            public FabricUiGridColumnSize? SizeXLarge { get; set; }
            public FabricUiGridColumnSize? SizeXXLarge { get; set; }
            public FabricUiGridColumnSize? SizeXXXLarge { get; set; }

            public FabricUiGridColumnSize? Push { get; set; }
            public FabricUiGridColumnSize? PushMedium { get; set; }
            public FabricUiGridColumnSize? PushLarge { get; set; }
            public FabricUiGridColumnSize? PushXLarge { get; set; }
            public FabricUiGridColumnSize? PushXXLarge { get; set; }
            public FabricUiGridColumnSize? PushXXXLarge { get; set; }

            public FabricUiGridColumnSize? Pull { get; set; }
            public FabricUiGridColumnSize? PullMedium { get; set; }
            public FabricUiGridColumnSize? PullLarge { get; set; }
            public FabricUiGridColumnSize? PullXLarge { get; set; }
            public FabricUiGridColumnSize? PullXXLarge { get; set; }
            public FabricUiGridColumnSize? PullXXXLarge { get; set; }

            public bool HiddenSmall { get; set; } = false;

            public bool HiddenMedium { get; set; } = false;
            public bool HiddenMediumAndLess { get; set; } = false;
            public bool HiddenMediumAndMore { get; set; } = false;

            public bool HiddenLarge { get; set; } = false;
            public bool HiddenLargeAndLess { get; set; } = false;
            public bool HiddenLargeAndMore { get; set; } = false;

            public bool HiddenXLarge { get; set; } = false;
            public bool HiddenXLargeAndLess { get; set; } = false;
            public bool HiddenXLargeAndMore { get; set; } = false;

            public bool HiddenXXLarge { get; set; } = false;
            public bool HiddenXXLargeAndLess { get; set; } = false;
            public bool HiddenXXLargeAndMore { get; set; } = false;

            public bool HiddenXXXLarge { get; set; } = false;

            public bool HasGreedyHeight { get; set; } = false;
        }
    }

    public enum FabricUiGridColumnSize
    {
        X1 = 1,
        X2 = 2,
        X3 = 3,
        X4 = 4,
        X5 = 5,
        X6 = 6,
        X7 = 7,
        X8 = 8,
        X9 = 9,
        X10 = 10,
        X11 = 11,
        X12 = 12,
    }
}
