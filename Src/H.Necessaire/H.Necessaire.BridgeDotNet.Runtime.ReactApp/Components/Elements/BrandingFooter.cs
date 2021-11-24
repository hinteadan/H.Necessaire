using Bridge.React;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class BrandingFooter : ComponentBase<BrandingFooter.Props, BrandingFooter.State>
    {
        public BrandingFooter() : base(new Props(), null) { }

        public override async Task RunAtStartup()
        {
            await base.RunAtStartup();
            await DoAndSetStateAsync(async x => x.AppVersion = await AppBase.GetAppVersion());
        }

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
                Config?.Get("App")?.Get("Copyright")?.ToString()?.Replace("{year}", DateTime.Today.Year.ToString())?.Replace("{version}", PrintAppVersion())
                ??
                $"Copyright &copy; {DateTime.Today.Year}. H.Necessaire by Hintea Dan Alexandru. All rights reserved. {PrintAppVersion()}"
                ;
        }

        private string PrintAppVersion()
        {
            return
                state.AppVersion?.Number?.Semantic != null
                ? $"v{state.AppVersion.Number.Semantic}"
                : string.Empty
                ;
        }

        public class State : ComponentStateBase
        {
            public Version AppVersion { get; set; } = null;
        }
        public class Props : ComponentPropsBase { }
    }
}
