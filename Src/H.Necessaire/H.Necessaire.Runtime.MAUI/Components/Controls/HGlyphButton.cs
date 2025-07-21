using H.Necessaire.Runtime.MAUI.Extensions;
using H.Necessaire.Runtime.MAUI.WellKnown;
using H.Necessaire.Runtime.MAUI.WellKnown.FluentUI;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HGlyphButton : ImageButton
#if ANDROID
        , IDisposable
#endif
    {
        string glyphName = null;
        readonly FontImageSource fontImageSource;
        public HGlyphButton()
        {
            fontImageSource = new FontImageSource
            {
                FontFamily = WellKnownTypography.FluentSystemIconsFilled,
                Glyph = null,
                Size = HUiToolkit.Current.Branding.Typography.FontSizeLarger,
                FontAutoScalingEnabled = true,
                Color = HUiToolkit.Current.Branding.ButtonTextColor.ToMaui(),
            };
            BackgroundColor = HUiToolkit.Current.Branding.PrimaryColor.ToMaui();
            Source = fontImageSource;
#if ANDROID
            Loaded += HGlyphButton_Loaded;
#endif
        }
#if ANDROID
        ~HGlyphButton() => HSafe.Run(Dispose);

        public void Dispose()
        {
            Loaded -= HGlyphButton_Loaded;
        }

        void HGlyphButton_Loaded(object sender, EventArgs e)
        {
            fontImageSource.Glyph = null;
            Glyph = Glyph;
        }
#endif

        public double GlyphSize
        {
            get => fontImageSource.Size;
            set => fontImageSource.Size = value;
        }

        public string Glyph
        {
            get => glyphName;
            set
            {
                glyphName = value;
                fontImageSource.Glyph = WellKnownFluentUiGlyphs.Glyph(glyphName);
            }
        }

        public Color GlyphColor
        {
            get => fontImageSource.Color;
            set => fontImageSource.Color = value;
        }
    }
}
