using Bridge;
using Bridge.React;
using System;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class FontIcon : PureComponent<FontIcon.Props>
    {
        public FontIcon(Props props) : base(props, null) { }

        public override ReactElement Render()
        {
            Attributes attributes = ApplyAriaAttributes(ConstructIconAttributesBasedOnProvider());

            attributes.Style = ConstructIconStyle();

            return
                DOM.I(attributes);
        }

        private Attributes ApplyAriaAttributes(Attributes attributes)
        {
            if (props.HasExternalLabel)
                attributes.Aria = new { hidden = true };
            else
                attributes.Aria = new { label = props.IconName };

            return attributes;
        }

        private ReactStyle ConstructIconStyle()
        {
            ReactStyle reactStyle = new ReactStyle { };
            if (props.Color != null)
                reactStyle.Color = props.Color;

            if (props.Size != null)
                reactStyle.FontSize = props.Size;

            if (props.StyleDecorator != null)
                reactStyle = props.StyleDecorator(reactStyle);

            return reactStyle;
        }

        private Attributes ConstructIconAttributesBasedOnProvider()
        {
            Attributes attributes;

            switch (props.Provider)
            {
                case Provider.FabricUI:
                    attributes = new Attributes { ClassName = $"ms-Icon ms-Icon--{props.IconName}" };
                    break;
                default:
                    attributes = new Attributes { };
                    break;
            }

            if (props.OnClick != null)
                attributes.OnClick = x => props.OnClick.Invoke();

            return attributes;
        }

        public class Props : ComponentPropsBase
        {
            public Provider Provider { get; set; } = Provider.FabricUI;
            public string IconName { get; set; } = null;
            public bool HasExternalLabel { get; set; } = false;
            public Func<ReactStyle, ReactStyle> StyleDecorator { get; set; }
            public string Color { get; set; } = Branding.TextColor.ToCssRGBA();
            public Union<string, int> Size { get; set; } = Branding.Typography.FontSize.PointsCss;
            public Action OnClick { get; set; }
        }

        public enum Provider
        {
            FabricUI = 0,
        }
    }
}
