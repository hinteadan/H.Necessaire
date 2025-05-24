using H.Necessaire.Runtime.MAUI.Extensions;
using H.Necessaire.Runtime.MAUI.WellKnown;
using H.Necessaire.Runtime.MAUI.WellKnown.FluentUI;

namespace H.Necessaire.Runtime.MAUI.Components.Controls
{
    public class HGlyphButton : ImageButton
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
                Color = HUiToolkit.Current.Branding.BackgroundColor.ToMaui(),
            };
            BackgroundColor = HUiToolkit.Current.Branding.PrimaryColor.ToMaui();
            Source = fontImageSource;
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
