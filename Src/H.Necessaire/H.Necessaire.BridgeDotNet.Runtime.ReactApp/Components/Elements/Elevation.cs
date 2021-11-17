using Bridge;
using Bridge.React;
using System;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class Elevation : ComponentBase<Elevation.Props, Elevation.State>
    {
        public Elevation(Props props, params Union<ReactElement, string>[] children) : base(props, children) { }
        public Elevation(params Union<ReactElement, string>[] children) : this(new Props { }, children) { }

        public override ReactElement Render()
        {
            ReactStyle reactStyle = new ReactStyle
            {
                MinWidth = 100,
                MinHeight = 60,
                BackgroundColor = Branding.BackgroundColor.ToCssRGBA(),
                MarginBottom = (int)props.Depth + Branding.SizingUnitInPixels,
            };

            if (props.StyleDecorator != null)
                reactStyle = props.StyleDecorator(reactStyle);

            return
                DOM.Div(
                    new Attributes
                    {
                        OnClick = x => props.OnClick?.Invoke(),
                        Style = reactStyle,
                        ClassName = $"ms-depth-{(int)props.Depth}{(string.IsNullOrWhiteSpace(props.ClassName) ? string.Empty : $" {props.ClassName}")}",
                    },

                    Children

                );
        }

        public class State : ComponentStateBase { }
        public class Props : ComponentPropsBase
        {
            public ElevationDepthLevel Depth { get; set; } = ElevationDepthLevel.Low;
            public Func<ReactStyle, ReactStyle> StyleDecorator { get; set; }
            public string ClassName { get; set; }
            public Action OnClick { get; set; }
        }
    }

    public enum ElevationDepthLevel
    {
        Lowest = 4,
        Low = 8,
        High = 16,
        Hihghest = 64,
    }
}
