using H.Necessaire.Models.Branding;

namespace H.Necessaire.Runtime.MAUI.Components
{
    internal class HUiToolkit
    {
        static readonly Lazy<HUiToolkit> current = new Lazy<HUiToolkit>(() => new HUiToolkit());
        public static HUiToolkit Current => current.Value;

        public HUiToolkit()
        {
            App = Application.Current.Handler.MauiContext.Services.GetService<HMauiApp>();
        }

        public HMauiApp App { get; }
        public int SizingUnit => App?.SizingUnit ?? 10;
        public BrandingStyle Branding => App?.Branding ?? HMauiAppBranding.Default;
    }
}
