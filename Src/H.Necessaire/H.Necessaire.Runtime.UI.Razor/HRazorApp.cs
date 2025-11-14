namespace H.Necessaire.Runtime.UI.Razor
{
    public class HRazorApp : HApp
    {
        public static readonly new HRazorApp Default = new HRazorApp();

        public HRazorApp()
        {
            Branding = HRazorAppBranding.Default;
        }
    }
}
