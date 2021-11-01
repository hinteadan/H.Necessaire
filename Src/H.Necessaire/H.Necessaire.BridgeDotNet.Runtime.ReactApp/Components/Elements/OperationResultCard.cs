using Bridge;
using Bridge.Html5;
using Bridge.React;
using System.Linq;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class OperationResultCard : ComponentBase<OperationResultCard.Props, OperationResultCard.State>
    {
        #region Construct
        public OperationResultCard(Props props) : base(props, null) { }
        #endregion

        public override ReactElement Render()
        {
            if (props?.OperationResult == null)
                return null;

            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            Display = Display.InlineBlock,
                            Width = props.Width,
                            Margin = Branding.SizingUnitInPixels,
                        },
                    },

                    RenderMessage(),
                    RenderAddendum()
                );
        }

        private ReactElement RenderMessage()
        {
            if (string.IsNullOrWhiteSpace(props.OperationResult.Reason))
                return null;

            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            FontSize = Branding.Typography.FontSizeSmaller.EmsCss,
                            Color = Branding.DangerColor.ToCssRGBA(),
                            Margin = $"{Branding.SizingUnitInPixels} {(Branding.SizingUnitInPixels)}",
                            Width = "100%",
                            TextAlign = props.TextAlign,
                        },
                    },
                    props.OperationResult.Reason
                );
        }

        private ReactElement RenderAddendum()
        {
            if (props.OperationResult.Comments == null)
                return null;

            if (props.OperationResult.Comments.All(x => string.IsNullOrWhiteSpace(x)))
                return null;

            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            FontSize = Branding.Typography.FontSizeSmaller.EmsCss,
                            Color = Branding.WarningColor.ToCssRGBA(),
                            Margin = $"{Branding.SizingUnitInPixels} {(Branding.SizingUnitInPixels)}",
                            Width = "100%",
                            TextAlign = props.TextAlign,
                        },
                    },
                    props
                        .OperationResult
                        .Comments
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(RenderAddendumEntry)
                        .ToArray()
                );

        }

        private ReactElement RenderAddendumEntry(string message)
        {
            return
                DOM.Div
                (
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            MarginTop = Branding.SizingUnitInPixels,
                            Width = "100%",
                            TextAlign = props.TextAlign,
                        },
                    },

                    message

                );
        }

        public class State : ComponentStateBase { }
        public class Props : ComponentPropsBase
        {
            public OperationResult OperationResult { get; set; }
            public Union<string, int> Width { get; set; } = 200;
            public TextAlign TextAlign { get; set; } = TextAlign.Left;
        }
    }
}
