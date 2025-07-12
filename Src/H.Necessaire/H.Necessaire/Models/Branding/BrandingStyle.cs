namespace H.Necessaire.Models.Branding
{
    public class BrandingStyle
    {
        public static readonly BrandingStyle Default = new BrandingStyle();

        public virtual int SizingUnitInPixels { get; } = 10;

        public virtual string DateFormat { get; } = "dd-MMM-yyyy";
        public virtual string TimeFormat { get; } = "HH:mm";
        public virtual string DateTimeFormat { get; } = "dd-MMM-yyyy HH:mm:ss";

        //public ColorPalette Colors { get; } = ColorPalette.Default;
        public virtual ColorPalette Colors { get; } = ColorPalette.CyanMate;

        //public Typography Typography { get; } = Typography.Default;
        public virtual Typography Typography { get; } = Typography.FiraSans;

        public virtual ColorInfo BackgroundColor => Colors.Complementary.Lighter(10);
        public virtual ColorInfo BackgroundColorTranslucent => Colors.Complementary.Lighter(10).Clone().And(x => x.Opacity = .83f);

        public virtual ColorInfo PrimaryColor => Colors.Primary.Color;
        public virtual ColorInfo PrimaryColorTranslucent => Colors.Primary.Lighter();
        public virtual ColorInfo PrimaryColorFaded => Colors.Primary.Lighter(2);

        public virtual ColorInfo SecondaryColor => Colors.PrimaryIsh().Color;
        public virtual ColorInfo SecondaryColorTranslucent => Colors.PrimaryIsh().Lighter();
        public virtual ColorInfo SecondaryColorFaded => Colors.PrimaryIsh().Lighter(2);

        public virtual ColorInfo TextColor { get; } = new ColorInfo(0, 0, 0);
        public virtual ColorInfo ButtonTextColor => Colors.Complementary.Lighter(10);
        public virtual ColorInfo LightTextColor { get; } = new ColorInfo(255, 255, 255, .85f);
        public virtual ColorInfo MutedTextColor { get; } = new ColorInfo(0, 0, 0, .35f);
        public virtual ColorInfo HighlightTextColor => Colors.Primary.Darker(10);

        public virtual ColorInfo SuccessColor { get; } = new ColorInfo("#11A74C");
        public virtual ColorInfo WarningColor { get; } = new ColorInfo("#EA850B");
        public virtual ColorInfo DangerColor { get; } = new ColorInfo("#CC2F2F");
        public virtual ColorInfo InformationColor { get; } = new ColorInfo("#0F957E");
        public virtual ColorInfo MuteColor { get; } = new ColorInfo(0, 0, 0, .5f);
    }
}
