using Bridge;
using Bridge.React;
using System.Linq;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class ListOfProperties : ComponentBase<ListOfProperties.Props, ListOfProperties.State>
    {
        public ListOfProperties(params Prop[] properties) : base(new Props { Properties = properties ?? new Prop[0] }, null) { }

        public override ReactElement Render()
        {
            return
                DOM.UL(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            Padding = 0,
                            Margin = $"{Branding.SizingUnitInPixels * 3 / 4}px 0 {Branding.SizingUnitInPixels}px 0",
                        }
                        .FlexNode(isVerticalFlow: true),
                    },
                    props.Properties?.Where(x => x != null).Select(x => DOM.Li(new LIAttributes
                    {
                        Style = new ReactStyle
                        {
                            MarginTop = Branding.SizingUnitInPixels * 1 / 4,
                        }
                        .FlexNode(),
                    }, RenderProperty(x)))
                );
        }

        private ReactElement RenderProperty(Prop property)
        {
            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle { }.FlexNode()
                    },

                    DOM.Div(new Attributes { Style = new ReactStyle { }.FlexNode() }, RenderName(property.Name)),
                    DOM.Div(new Attributes { Style = new ReactStyle { }.FlexNode() }, RenderValue(property.Value))

                );
        }

        private ReactElement RenderName(Union<ReactElement, string> name)
        {
            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            Color = Branding.Colors.Primary.Color.ToCssRGBA(),
                            FontWeight = "bold",
                            JustifyContent = Bridge.Html5.JustifyContent.FlexEnd,
                            PaddingRight = Branding.SizingUnitInPixels / 2,
                        }.FlexNode(),
                    },

                    name

                );
        }

        private ReactElement RenderValue(Union<ReactElement, string> value)
        {
            return
                DOM.Div(
                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            Color = Branding.Colors.Complementary.Color.ToCssRGBA(),
                            JustifyContent = Bridge.Html5.JustifyContent.FlexStart,
                            PaddingLeft = Branding.SizingUnitInPixels / 2,
                        }.FlexNode(),
                    },

                    value

                );
        }

        public class State : ComponentStateBase { }
        public class Props : ComponentPropsBase
        {
            public Prop[] Properties { get; set; } = new Prop[0];
        }

        public class Prop
        {
            public Union<ReactElement, string> Name { get; set; } = null;
            public Union<ReactElement, string> Value { get; set; } = null;
        }
    }
}
