using Bridge;
using Bridge.Html5;
using Bridge.React;
using System;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class Button : ComponentBase<Button.Props, Button.State>
    {
        public Button(Props props, params Union<ReactElement, string>[] children) : base(props, children) { }
        public Button(params Union<ReactElement, string>[] children) : this(new Props { }, children) { }

        public override ReactElement Render()
        {
            ReactStyle reactStyle = new ReactStyle
            {
                BorderRadius = 0,
                Border = "none",
                Margin = 0,
                Padding = "10px 30px",
                BackgroundColor = GetBackgroundColor(),
                Color = GetTextColor(),
                Cursor = props.IsDisabled ? Cursor.NotAllowed : Cursor.Pointer,
                Outline = "none",
                FontFamily = Branding.Typography.FontFamily,
                FontSize = Branding.Typography.FontSize.PointsCss,
                Opacity = props.IsDisabled ? "0.25" : null,
                MinWidth = props.MinWidth,
                Width = props.Width,
                Height = props.Height,
                TextAlign = TextAlign.Center,
                JustifyContent = JustifyContent.Center,
            };

            if (props.StyleDecorator != null)
                reactStyle = props.StyleDecorator(reactStyle);

            ButtonAttributes attributes
                = new ButtonAttributes
                {

                    Style = reactStyle,

                    Disabled = props.IsDisabled,

                    OnMouseOver = x => { EnableOnHoverStyle(x.CurrentTarget); },
                    OnMouseOut = x => { DisableOnHoverStyle(x.CurrentTarget); },
                    OnMouseDown = x => { EnableOnPressedStyle(x.CurrentTarget); },
                    OnTouchStart = x => { EnableOnPressedStyle(x.CurrentTarget); },
                    OnMouseUp = x => { DisableOnPressedStyle(x.CurrentTarget); },
                    OnTouchEnd = x => { DisableOnPressedStyle(x.CurrentTarget); },

                    OnClick = x => props.OnClick?.Invoke(),

                };

            if (props.Decorator != null) attributes = props.Decorator.Invoke(attributes);

            return
                DOM.Button(attributes, Children);
        }

        private void EnableOnPressedStyle(HTMLButtonElement buttonElement)
        {
            if (props.IsDisabled) return;

            buttonElement.Style.BackgroundColor = GetBackgroundPressedColor();
        }

        private void DisableOnPressedStyle(HTMLButtonElement buttonElement)
        {
            EnableOnHoverStyle(buttonElement);
            //buttonElement.Style.BackgroundColor = GetBackgroundColor();
        }

        private void EnableOnHoverStyle(HTMLButtonElement buttonElement)
        {
            if (props.IsDisabled) return;

            buttonElement.Style.BackgroundColor = GetBackgroundHoverColor();
        }

        private void DisableOnHoverStyle(HTMLButtonElement buttonElement)
        {
            buttonElement.Style.BackgroundColor = GetBackgroundColor();
        }

        private string GetTextColor()
        {
            switch (props.Role)
            {
                case ButtonRole.Negative:
                case ButtonRole.Default:
                default: return Branding.BackgroundColor.ToCssRGBA();
            }
        }

        private string GetBackgroundColor()
        {
            switch (props.Role)
            {
                case ButtonRole.Negative: return Branding.Colors.Primary.Lighter(3).ToCssRGBA();
                case ButtonRole.Default:
                default: return Branding.Colors.Primary.Color.ToCssRGBA();
            }
        }

        private string GetBackgroundHoverColor()
        {
            switch (props.Role)
            {
                case ButtonRole.Negative: return Branding.Colors.Primary.Lighter(4).ToCssRGBA();
                case ButtonRole.Default:
                default: return Branding.Colors.Primary.Lighter().ToCssRGBA();
            }
        }

        private string GetBackgroundPressedColor()
        {
            switch (props.Role)
            {
                case ButtonRole.Negative: return Branding.Colors.Primary.Lighter(5).ToCssRGBA();
                case ButtonRole.Default:
                default: return Branding.Colors.Primary.Darker().ToCssRGBA();
            }
        }

        public class State : ComponentStateBase { }
        public class Props : ComponentPropsBase
        {
            public Action OnClick { get; set; }
            public ButtonRole Role { get; set; } = ButtonRole.Default;
            public bool IsDisabled { get; set; } = false;
            public Union<string, int> Width { get; set; } = null;
            public Union<string, int> MinWidth { get; set; } = 200;
            public Union<string, int> Height { get; set; } = null;
            public Func<ButtonAttributes, ButtonAttributes> Decorator { get; set; }
            public Func<ReactStyle, ReactStyle> StyleDecorator { get; set; }
        }
    }

    public enum ButtonRole
    {
        Default = 0,
        Negative = 10,
    }
}
