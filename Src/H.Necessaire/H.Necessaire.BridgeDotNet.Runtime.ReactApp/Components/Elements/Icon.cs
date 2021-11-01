using Bridge;
using Bridge.React;
using System;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class Icon : PureComponent<Icon.Props>
    {
        #region Construct
        public Icon(Props props) : base(props, null) { }
        #endregion

        public override ReactElement Render()
        {
            string imageUrl = $"url('{props.Url}')";

            ReactStyle style = new ReactStyle
            {
                Display = Bridge.Html5.Display.InlineBlock,
                BackgroundImage = imageUrl,
                BackgroundRepeat = Bridge.Html5.BackgroundRepeat.NoRepeat,
                BackgroundPosition = "center center",
                BackgroundSize = "contain",
                Width = props.Width,
                Height = props.Height,
                Margin = props.Margin,
            };

            if (props.OnClick != null)
                style.Cursor = Bridge.Html5.Cursor.Pointer;

            if (!string.IsNullOrWhiteSpace(props.TintColor))
            {
                style.BackgroundImage = "none";
                style.BackgroundColor = props.TintColor;
                style.BackgroundBlendMode = Bridge.Html5.BackgroundBlendMode.Overlay;

                style["mask-image"] = imageUrl;
                style["-webkit-mask-image"] = imageUrl;
                style["mask-repeat"] = "no-repeat";
                style["-webkit-mask-repeat"] = "no-repeat";
                style["mask-size"] = "contain";
                style["-webkit-mask-size"] = "contain";
                style["mask-position"] = "center center";
                style["-webkit-mask-position"] = "center center";
            }

            return
                DOM.Div(
                    new Attributes
                    {
                        Style = style,
                        OnClick = x => props?.OnClick?.Invoke(),
                    }
                );
        }

        public class Props : ComponentPropsBase
        {
            public string Url { get; set; }
            public string TintColor { get; set; } = null;
            public Union<string, int> Width { get; set; } = 40;
            public Union<string, int> Height { get; set; } = 40;
            public Union<string, int> Margin { get; set; } = 0;
            public Action OnClick { get; set; }
        }
    }
}
