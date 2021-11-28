namespace H.Necessaire.Models.Branding
{
    public class BrandingStyle
    {
        public static readonly BrandingStyle Default = new BrandingStyle();

        #region Construct
        public virtual int SizingUnitInPixels { get;  } = 10;

        //public ColorPalette Colors { get; } = ColorPalette.Default;
        public virtual ColorPalette Colors { get;  } = ColorPalette.CyanMate;

        //public Typography Typography { get; } = Typography.Default;
        public virtual Typography Typography { get; } = Typography.FiraSans;
        #endregion

        public virtual ColorInfo BackgroundColor => Colors.Complementary.Lighter(10);
        public virtual ColorInfo BackgroundColorTranslucent => Colors.Complementary.Lighter(10).Clone().And(x => x.Opacity = .83f);

        public virtual ColorInfo PrimaryColor => Colors.Primary.Color;
        public virtual ColorInfo PrimaryColorTranslucent => Colors.Primary.Lighter();
        public virtual ColorInfo PrimaryColorFaded => Colors.Primary.Lighter(2);

        public virtual ColorInfo SecondaryColor => Colors.PrimaryIsh().Color;
        public virtual ColorInfo SecondaryColorTranslucent => Colors.PrimaryIsh().Lighter();
        public virtual ColorInfo SecondaryColorFaded => Colors.PrimaryIsh().Lighter(2);

        public virtual ColorInfo TextColor { get; } = new ColorInfo(0, 0, 0);
        public virtual ColorInfo LightTextColor { get; } = new ColorInfo(255, 255, 255, .85f);
        public virtual ColorInfo MutedTextColor { get; } = new ColorInfo(0, 0, 0, .35f);
        public virtual ColorInfo HighlightTextColor => Colors.Primary.Darker(10);

        public virtual ColorInfo SuccessColor { get; } = new ColorInfo("#0b6b31");
        public virtual ColorInfo WarningColor { get; } = new ColorInfo("#b36f1d");
        public virtual ColorInfo DangerColor { get; } = new ColorInfo("#911717");
        public virtual ColorInfo InformationColor { get; } = new ColorInfo("#0e6355");
        public virtual ColorInfo MuteColor { get; } = new ColorInfo(0, 0, 0, .5f);
    }
}
