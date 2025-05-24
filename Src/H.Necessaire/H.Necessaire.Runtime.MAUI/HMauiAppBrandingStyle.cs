using H.Necessaire.Models.Branding;
using H.Necessaire.Runtime.MAUI.WellKnown;
using H.Necessaire.Runtime.UI;

namespace H.Necessaire.Runtime.MAUI
{
    public class HMauiAppBranding : BrandingStyle
    {
        public static readonly new HMauiAppBranding Default = new HMauiAppBranding();

        public override int SizingUnitInPixels => 14;

        public AppTheme Theme => GetCurrentTheme();

        public override ColorPalette Colors => HAppColorPalette.LightAzure;

        public override Typography Typography => WellKnownTypography.RobotoCondensedTypography;

        public override ColorInfo BackgroundColor => Theme == AppTheme.Dark ? Colors.Complementary.Darker(10) : base.BackgroundColor;
        public override ColorInfo BackgroundColorTranslucent => Theme == AppTheme.Dark ? Colors.Complementary.Darker(10).WithOpacity(.83f) : base.BackgroundColorTranslucent;

        public override ColorInfo PrimaryColor => Theme == AppTheme.Dark ? Colors.Primary.Darker(1) : base.PrimaryColor;
        public override ColorInfo PrimaryColorTranslucent => Theme == AppTheme.Dark ? Colors.Primary.Darker(1).WithOpacity(.83f) : base.PrimaryColorTranslucent;
        public override ColorInfo PrimaryColorFaded => Theme == AppTheme.Dark ? Colors.Primary.Color.WithOpacity(.83f) : base.PrimaryColorFaded;

        public override ColorInfo SecondaryColor => Theme == AppTheme.Dark ? Colors.PrimaryIsh().Darker(1) : base.SecondaryColor;
        public override ColorInfo SecondaryColorTranslucent => Theme == AppTheme.Dark ? Colors.PrimaryIsh().Darker(1).WithOpacity(.83f) : base.SecondaryColorTranslucent;
        public override ColorInfo SecondaryColorFaded => Theme == AppTheme.Dark ? Colors.PrimaryIsh().Color.WithOpacity(.83f) : base.SecondaryColorFaded;

        public override ColorInfo TextColor => Theme == AppTheme.Dark ? new ColorInfo(255, 255, 255, .83f) : base.TextColor;
        public override ColorInfo ButtonTextColor => Theme == AppTheme.Dark ? new ColorInfo(255, 255, 255, .83f).WithOpacity(.83f) : base.ButtonTextColor;
        public override ColorInfo LightTextColor => Theme == AppTheme.Dark ? new ColorInfo(0, 0, 0, .85f) : base.LightTextColor;
        public override ColorInfo MutedTextColor => Theme == AppTheme.Dark ? new ColorInfo(0, 0, 0, .35f) : base.MutedTextColor;
        public override ColorInfo HighlightTextColor => Theme == AppTheme.Dark ? Colors.Primary.Lighter(4) : base.HighlightTextColor;

        private static AppTheme GetCurrentTheme()
        {
            AppTheme? theme = Application.Current?.UserAppTheme;
            if (theme is not null && theme != AppTheme.Unspecified)
                return theme.Value;

            return Application.Current?.RequestedTheme ?? AppTheme.Unspecified;
        }
    }
}
