using Bridge;
using Bridge.Html5;
using Bridge.React;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class TextInput : ComponentBase<TextInput.Props, TextInput.State>
    {
        public TextInput(Props props) : base(props, null) { }
        public TextInput() : this(new Props { }) { }

        public override ReactElement Render()
        {
            var attributes
                = new InputAttributes
                {
                    Style = new ReactStyle
                    {
                        Display = Display.Block,
                        Border = "solid 1px",
                        BorderColor = GetBorderColor(),
                        Margin = "0 -20px 0 0",
                        Padding = 10,
                        BackgroundColor = Branding.BackgroundColor.ToCssRGBA(),
                        Color = Branding.TextColor.ToCssRGBA(),
                        FontFamily = Branding.Typography.FontFamily,
                        FontSize = Branding.Typography.FontSize.PointsCss,
                        Outline = "none",
                        Width = "100%",
                        BoxSizing = BoxSizing.BorderBox,
                    },

                    Placeholder = string.IsNullOrWhiteSpace(props.Placeholder) ? null : props.Placeholder,

                    Disabled = props.IsDisabled,

                    OnFocus = x => { EnableOnFocusedStyle(x.CurrentTarget); },
                    OnBlur = async x => { DisableOnFocusedStyle(x.CurrentTarget); await ValidateUserInput(x.CurrentTarget.Value); },

                    OnChange = x => ValidateUserInputWhileTyping(x.CurrentTarget.Value),
                };

            if (props.Decorator != null) attributes = props.Decorator.Invoke(attributes);

            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            Display = Display.InlineBlock,
                            Width = props.Width,
                            Opacity = props.IsDisabled ? "0.25" : null,
                        },
                    },
                    RenderLabel(),
                    DOM.Div(new Attributes { Style = new ReactStyle { Width = "100%", Position = Position.Relative } },
                        DOM.Input(attributes),
                        RenderHighlight()
                    ),
                    RenderDescription()
                );
        }

        private void ValidateUserInputWhileTyping(string value)
        {
            if (props.UserInputValidator == null)
                return;

            CancelUserInputValidation();

            state.validateUserInputWhileTypingTimeoutID = Window.SetTimeout(async () => { await ValidateUserInput(value); state.validateUserInputWhileTypingTimeoutID = -1; }, (int)state.idleTimeToConsiderUserTypingIsFinished.TotalMilliseconds);
        }

        private async Task ValidateUserInput(string value)
        {
            if (props.UserInputValidator == null)
                return;

            CancelUserInputValidation();

            using (state.validateUserInputWhileTypingCancelTokenSource = new CancellationTokenSource())
            {
                try
                {
                    state.ValidationState = await props.UserInputValidator.Invoke(value, state.validateUserInputWhileTypingCancelTokenSource.Token);
                    await SetStateAsync();
                }
                finally { }
            }
            state.validateUserInputWhileTypingCancelTokenSource = null;


        }

        private void CancelUserInputValidation()
        {
            if (state.validateUserInputWhileTypingCancelTokenSource != null)
            {
                try { state.validateUserInputWhileTypingCancelTokenSource.Cancel(); } finally { }
            }

            if (state.validateUserInputWhileTypingTimeoutID >= 0)
            {
                Window.ClearTimeout(state.validateUserInputWhileTypingTimeoutID);
                state.validateUserInputWhileTypingTimeoutID = -1;
            }
        }

        private ReactElement RenderHighlight()
        {
            if (!props.IsHighlighted) return null;

            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            Height = 8,
                            Width = 8,
                            Position = Position.Absolute,
                            Bottom = 0,
                            Right = 0,
                            BackgroundColor = GetHighlightColor(),
                        }
                    }
                );
        }

        private string GetHighlightColor()
        {
            if (state?.ValidationState?.IsSuccessful == null)
                return Branding.Colors.Primary.Lighter(2).ToCssRGBA();

            return
                state.ValidationState.IsSuccessful == true
                ? Branding.SuccessColor.ToCssRGBA()
                : Branding.DangerColor.ToCssRGBA()
                ;
        }

        private ReactElement RenderDescription()
        {
            if (string.IsNullOrWhiteSpace(props.Description))
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
                        },
                    },

                    string.IsNullOrWhiteSpace(state?.ValidationState?.Reason) ? props.Description : state.ValidationState.Reason

                );
        }

        private ReactElement RenderLabel()
        {
            if (string.IsNullOrWhiteSpace(props.Label))
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
                        },
                    },
                    props.Label
                );
        }

        private void EnableOnFocusedStyle(HTMLInputElement inputElement)
        {
            if (props.IsDisabled) return;

            inputElement.Style.BorderColor = Branding.Colors.Primary.Color.ToCssRGBA();
        }

        private void DisableOnFocusedStyle(HTMLInputElement inputElement)
        {
            inputElement.Style.BorderColor = GetBorderColor();
        }

        private string GetBorderColor()
        {
            return Branding.Colors.Primary.Lighter(2).ToCssRGBA();
        }

        public class State : ComponentStateBase
        {
            public TimeSpan idleTimeToConsiderUserTypingIsFinished { get; } = TimeSpan.FromMilliseconds(500);
            public int validateUserInputWhileTypingTimeoutID = -1;
            public CancellationTokenSource validateUserInputWhileTypingCancelTokenSource = null;

            public OperationResult ValidationState { get; set; }
        }
        public class Props : ComponentPropsBase
        {
            public bool IsDisabled { get; set; } = false;
            public Func<InputAttributes, InputAttributes> Decorator { get; set; }
            public string Label { get; set; }
            public string Placeholder { get; set; }
            public string Description { get; set; }
            public Union<string, int> Width { get; set; } = 200;
            public Func<string, CancellationToken, Task<OperationResult>> UserInputValidator { get; set; }
            public bool IsHighlighted => UserInputValidator != null;
        }
    }
}
