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

            FontSizeLarge = new TypographySize().ScaleBy(1.2f);
            FontSizeLarger = new TypographySize().ScaleBy(1.4f);

            FontSizeSmall = new TypographySize().ScaleBy(.8f);
            FontSizeSmaller = new TypographySize().ScaleBy(.6f);
        }

        public static readonly Typography Default = new Typography(
            "'Roboto Condensed', 'Roboto', Calibri, Helvetica, Arial, Sans-Serif",
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
