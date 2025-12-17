using H.Necessaire.Runtime.UI.Razor;

namespace H.Necessaire.AspNetCoreWebAppSample
{
    public class HAspNetCoreSampleAppBranding : HRazorAppBranding
    {
        static readonly Typography typography = new Typography(
            "'Roboto Condensed', 'Roboto', Calibri, Helvetica, Arial, Sans-Serif",
            new TypographySize(12),
            "https://fonts.googleapis.com/css2?family=Roboto+Condensed:ital,wght@0,300;0,400;0,700;1,300;1,400;1,700&display=swap",
            "https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap&subset=latin-ext"
        );
        public static readonly new HAspNetCoreSampleAppBranding Default = new HAspNetCoreSampleAppBranding();

        public override Typography Typography => typography;

        public override ColorInfo BackgroundColorTranslucent => base.BackgroundColor.WithOpacity(.925f);
    }
}
