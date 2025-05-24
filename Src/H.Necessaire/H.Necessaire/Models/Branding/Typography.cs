namespace H.Necessaire
{
    public class Typography
    {
        private Typography() { }
        public Typography(string fontFamily, TypographySize fontSize, params string[] fontFamilyUrls)
        {
            FontFamily = fontFamily;
            FontFamilyUrls = fontFamilyUrls ?? new string[0];
            FontSize = fontSize;

            FontSizeLarge = fontSize.Points * 1.2f;
            FontSizeLarger = fontSize.Points * 1.4f;

            FontSizeSmall = fontSize.Points * .8f;
            FontSizeSmaller = fontSize.Points * .6f;
        }

        public static readonly Typography FiraSans = new Typography(
            "'Fira Sans', Helvetica, Calibri, Arial, Sans-Serif",
            new TypographySize(12),
            "https://fonts.googleapis.com/css2?family=Fira+Sans:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap"
            );

        public static readonly Typography Default = new Typography(
            "'Roboto', 'Roboto Condensed', Calibri, Helvetica, Arial, Sans-Serif",
            new TypographySize(12),
            "https://fonts.googleapis.com/css2?family=Roboto+Condensed:ital,wght@0,300;0,400;0,700;1,300;1,400;1,700&display=swap",
            "https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap&subset=latin-ext"
            );

        public string FontFamily { get; }
        public string[] FontFamilyUrls { get; }
        public TypographySize FontSize { get; }

        public TypographySize FontSizeLarge { get; }
        public TypographySize FontSizeLarger { get; }
        public TypographySize FontSizeSmall { get; }
        public TypographySize FontSizeSmaller { get; }
    }
}
