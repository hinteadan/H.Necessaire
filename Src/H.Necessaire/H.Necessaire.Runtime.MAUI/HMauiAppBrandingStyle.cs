using H.Necessaire.Models.Branding;
using H.Necessaire.Runtime.MAUI.WellKnown;
using H.Necessaire.Runtime.UI;

namespace H.Necessaire.Runtime.MAUI
{
    public class HMauiAppBranding : BrandingStyle
    {
        public static readonly new HMauiAppBranding Default = new HMauiAppBranding();

        public override int SizingUnitInPixels => 14;

        public override ColorPalette Colors => HAppColorPalette.LightAzure;

        public override Typography Typography => WellKnownTypography.RobotoCondensedTypography;
    }
}
