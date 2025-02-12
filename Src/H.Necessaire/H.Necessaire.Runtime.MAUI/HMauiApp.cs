using H.Necessaire.Runtime.UI;

namespace H.Necessaire.Runtime.MAUI
{
    public class HMauiApp : HApp
    {
        public static readonly new HMauiApp Default = new HMauiApp();

        public HMauiApp()
        {
            Branding = HMauiAppBranding.Default;
        }
    }
}
