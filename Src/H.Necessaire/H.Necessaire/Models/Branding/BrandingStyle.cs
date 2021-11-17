namespace H.Necessaire.Models.Branding
{
    public class BrandingStyle
    {
        public static readonly BrandingStyle Default = new BrandingStyle();

        #region Construct
        public readonly int SizingUnitInPixels = 10;

        //public ColorPalette Colors { get; } = ColorPalette.Default;
        public ColorPalette Colors { get; } = ColorPalette.CyanMate;

        //public Typography Typography { get; } = Typography.Default;
        public Typography Typography { get; } = Typography.FiraSans;
        #endregion

        public ColorInfo BackgroundColor => Colors.Complementary.Lighter(10);
        public ColorInfo BackgroundColorTranslucent => Colors.Complementary.Lighter(10).Clone().And(x => x.Opacity = .83f);

        public ColorInfo PrimaryColor => Colors.Primary.Color;
        public ColorInfo PrimaryColorTranslucent => Colors.Primary.Lighter();
        public ColorInfo PrimaryColorFaded => Colors.Primary.Lighter(2);

        public ColorInfo SecondaryColor => Colors.PrimaryIsh().Color;
        public ColorInfo SecondaryColorTranslucent => Colors.PrimaryIsh().Lighter();
        public ColorInfo SecondaryColorFaded => Colors.PrimaryIsh().Lighter(2);

        public ColorInfo TextColor { get; } = new ColorInfo(0, 0, 0);
        public ColorInfo LightTextColor { get; } = new ColorInfo(255, 255, 255, .85f);
        public ColorInfo MutedTextColor { get; } = new ColorInfo(0, 0, 0, .35f);
        public ColorInfo HighlightTextColor => Colors.Primary.Darker(10);

        public ColorInfo SuccessColor { get; } = new ColorInfo("#0b6b31");
        public ColorInfo WarningColor { get; } = new ColorInfo("#b36f1d");
        public ColorInfo DangerColor { get; } = new ColorInfo("#911717");
        public ColorInfo InformationColor { get; } = new ColorInfo("#0e6355");
        public ColorInfo MuteColor { get; } = new ColorInfo(0, 0, 0, .5f);
    }
}
