using Bridge;
using Bridge.Html5;
using Bridge.React;
using System.Linq;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class ValidationResultDisplay : ComponentBase<ValidationResultDisplay.Props, ValidationResultDisplay.State>
    {
        public ValidationResultDisplay(Props props) : base(props, null) { }
        public ValidationResultDisplay() : this(new Props { }) { }

        public override ReactElement Render()
        {
            if (props?.ValidationResult == null)
                return null;

            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            Display = Display.InlineBlock,
                            Width = props.Width,
                        },
                    },

                    RenderMessage(),
                    RenderAddendum()
                );
        }

        private ReactElement RenderMessage()
        {
            if (string.IsNullOrWhiteSpace(props.ValidationResult.Reason))
                return null;

            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            FontSize = Branding.Typography.FontSizeSmaller.PointsCss,
                            Color = Branding.Colors.Primary.Color.ToCssRGBA(),
                            Margin = "2px 1px",
                            Width = "100%",
                            TextAlign = props.TextAlign,
                        },
                    },
                    props.ValidationResult.Reason
                );
        }

        private ReactElement RenderAddendum()
        {
            if (props.ValidationResult.Comments == null)
                return null;

            if (props.ValidationResult.Comments.All(x => string.IsNullOrWhiteSpace(x)))
                return null;

            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            FontSize = Branding.Typography.FontSizeSmaller.PointsCss,
                            Color = Branding.MutedTextColor.ToCssRGBA(),
                            Margin = "2px 1px",
                            Width = "100%",
                            TextAlign = props.TextAlign,
                        },
                    },
                    props
                        .ValidationResult
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
                            MarginTop = 2,
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
            public OperationResult ValidationResult { get; set; }
            public Union<string, int> Width { get; set; } = 200;
            public TextAlign TextAlign { get; set; } = TextAlign.Left;
        }
    }
}
