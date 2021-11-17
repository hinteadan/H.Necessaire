using Bridge;
using Bridge.Html5;
using Bridge.React;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class Subtitle : ComponentBase<Subtitle.Props, Subtitle.State>
    {
        public Subtitle(Props props, params Union<ReactElement, string>[] children) : base(props, children) { }
        public Subtitle(params Union<ReactElement, string>[] children) : this(new Props { }, children) { }

        public override ReactElement Render()
        {
            return
                DOM.H2(

                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            Color = Branding.Colors.Primary.Lighter().ToCssRGBA(),
                            FontSize = Branding.Typography.FontSizeLarge.EmsCss,
                            TextAlign = props.TextAlign,
                        },
                    },

                    Children

                );
        }

        public class State : ComponentStateBase { }
        public class Props : ComponentPropsBase
        {
            public TextAlign TextAlign { get; set; } = TextAlign.Center;
        }
    }
}
