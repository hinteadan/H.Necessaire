using Bridge.React;
using System;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class BrandingFooter : ComponentBase<BrandingFooter.Props, BrandingFooter.State>
    {
        public BrandingFooter() : base(new Props(), null) { }

        public override ReactElement Render()
        {
            return
                DOM.Footer(

                    new Attributes
                    {
                        Style = new ReactStyle
                        {
                            Height = Branding.SizingUnitInPixels * 3,
                            Display = Bridge.Html5.Display.Flex,
                            FlexDirection = Bridge.Html5.FlexDirection.Column,
                            JustifyContent = Bridge.Html5.JustifyContent.Center,
                            TextAlign = Bridge.Html5.TextAlign.Center,
                            Color = Branding.MuteColor.ToCssRGBA(),
                            FontSize = Branding.Typography.FontSizeSmaller.EmsCss,
                            BackgroundColor = Branding.SecondaryColorTranslucent.ToCssRGBA(),
                            BoxShadow = "0 -3.2px 7.2px 0 rgba(0,0,0,.132),0 .6px 1.8px 0 rgba(0,0,0,.108)",
                        },
                    },

                    DOM.Div(
                        new Attributes { },
                        DOM.Span(new Attributes { DangerouslySetInnerHTML = new RawHtml { Html = RenderCopyrightLabel() } })
                    )

                );
        }

        private string RenderCopyrightLabel()
        {
            return
                Config?.Get("App")?.Get("Copyright")?.ToString()?.Replace("{year}", DateTime.Today.Year.ToString())
                ??
                $"Copyright &copy; {DateTime.Today.Year}. H.Necessaire by Hintea Dan Alexandru. All rights reserved."
                ;
        }

        public class State : ComponentStateBase { }
        public class Props : ComponentPropsBase { }
    }
}
