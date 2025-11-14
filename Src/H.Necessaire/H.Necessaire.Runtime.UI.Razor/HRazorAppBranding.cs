using H.Necessaire.Models.Branding;

namespace H.Necessaire.Runtime.UI.Razor
{
    public class HRazorAppBranding : BrandingStyle
    {
        public static readonly new HRazorAppBranding Default = new HRazorAppBranding();

        //public override int SizingUnitInPixels => 10;

        public override ColorPalette Colors => HAppColorPalette.LightAzure;

        public override Typography Typography => Typography.Default;
    }
}
