using H.Necessaire.Runtime.UI.Razor;

namespace H.Necessaire.AspNetCoreWebAppSample
{
    public class HAspNetCoreSampleApp : HRazorApp
    {
        public static readonly HAspNetCoreSampleApp Instance = new HAspNetCoreSampleApp();

        public HAspNetCoreSampleApp()
        {
            Branding = HAspNetCoreSampleAppBranding.Default;
        }
    }
}
