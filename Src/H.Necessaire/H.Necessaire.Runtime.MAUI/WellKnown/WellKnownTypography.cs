using System.Reflection;

namespace H.Necessaire.Runtime.MAUI.WellKnown
{
    public static class WellKnownTypography
    {
        static int defaultFontSizeInPoints = 16;

        public const string Roboto = "Roboto";
        public static readonly Typography RobotoTypography = new Typography(
            Roboto,
            new TypographySize(defaultFontSizeInPoints),
            "Roboto-Regular.ttf"
        );

        public const string RobotoCondensed = "RobotoCondensed";
        public static readonly Typography RobotoCondensedTypography = new Typography(
            RobotoCondensed,
            new TypographySize(defaultFontSizeInPoints),
            "Roboto_Condensed-Regular.ttf"
        );

        public const string FluentSystemIconsFilled = "FluentSystemIconsFilled";
        public static readonly Typography FluentSystemIconsFilledTypography = new Typography(
            FluentSystemIconsFilled,
            new TypographySize(defaultFontSizeInPoints),
            "FluentSystemIcons-Filled.ttf"
        );

        public const string FluentSystemIconsLight = "FluentSystemIconsLight";
        public static readonly Typography FluentSystemIconsLightTypography = new Typography(
            FluentSystemIconsLight,
            new TypographySize(defaultFontSizeInPoints),
            "FluentSystemIcons-Light.ttf"
        );

        public const string FluentSystemIconsRegular = "FluentSystemIconsRegular";
        public static readonly Typography FluentSystemIconsRegularTypography = new Typography(
            FluentSystemIconsRegular,
            new TypographySize(defaultFontSizeInPoints),
            "FluentSystemIcons-Regular.ttf"
        );

        public const string FluentSystemIconsResizable = "FluentSystemIconsResizable";
        public static readonly Typography FluentSystemIconsResizableTypography = new Typography(
            FluentSystemIconsResizable,
            new TypographySize(defaultFontSizeInPoints),
            "FluentSystemIcons-Resizable.ttf"
        );


        static readonly Lazy<Typography[]> allWellKnownTypographies = new Lazy<Typography[]>(BuildAllWellKnownTypographies);
        public static Typography[] All => allWellKnownTypographies.Value;

        static Typography[] BuildAllWellKnownTypographies()
        {
            return
                typeof(WellKnownTypography)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(Typography))
                .Select(f => f.GetValue(null) as Typography)
                .ToArray()
                ;
        }
    }
}
