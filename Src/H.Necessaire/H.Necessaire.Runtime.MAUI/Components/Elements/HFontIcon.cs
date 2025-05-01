using H.Necessaire.Runtime.MAUI.Components.Abstracts;
using H.Necessaire.Runtime.MAUI.Extensions;
using H.Necessaire.Runtime.MAUI.WellKnown;
using H.Necessaire.Runtime.MAUI.WellKnown.FluentUI;

namespace H.Necessaire.Runtime.MAUI.Components.Elements
{
    public class HFontIcon : HMauiComponentBase
    {
        FontImageSource fontImageSource;
        Image fontImage;
        protected override View ConstructContent() => ConstructFontIcon();

        View ConstructFontIcon()
        {
            fontImageSource = new FontImageSource
            {
                FontFamily = WellKnownTypography.FluentSystemIconsFilled,
                Glyph = null,
                Size = Branding.SizingUnitInPixels * 20,
                FontAutoScalingEnabled = true,
                Color = Branding.TextColor.ToMaui(),
            };

            fontImage = new Image
            {
                Source = fontImageSource,
            };

            return fontImage;
        }

        public double Size
        {
            get => fontImageSource.Size;
            set => fontImageSource.Size = value;
        }

        public Color Color
        {
            get => fontImageSource.Color;
            set => fontImageSource.Color = value;
        }

        public Image IconImage => fontImage;

        string variant = WellKnownFluentUiGlyphVariant.Filled;
        /// <summary>
        /// Values from WellKnownFluentUiGlyphVariant, any other value will be ignored and replaced by the default value, "Filled"
        /// </summary>
        public string Variant
        {
            get => variant;
            set => variant = SetVariant(value);
        }
        public string Glyph
        {
            get => fontImageSource.Glyph;
            set => SetGlyph(value);
        }
        public HFontIcon SetGlyph(string glyphName, string variant = null)
        {
            if (variant is not null)
                Variant = variant;

            fontImageSource.Glyph = WellKnownFluentUiGlyphs.Glyph(glyphName, Variant);

            return this;
        }
        string SetVariant(string variant)
        {
            variant = WellKnownFluentUiGlyphVariant.GetVariantOrDefault(variant);

            if (this.variant.Is(variant))
                return this.variant;

            switch (variant)
            {
                case WellKnownFluentUiGlyphVariant.Light:
                    fontImageSource.FontFamily = WellKnownTypography.FluentSystemIconsLight;
                    break;
                case WellKnownFluentUiGlyphVariant.Regular:
                    fontImageSource.FontFamily = WellKnownTypography.FluentSystemIconsRegular;
                    break;
                case WellKnownFluentUiGlyphVariant.Resizable:
                    fontImageSource.FontFamily = WellKnownTypography.FluentSystemIconsResizable;
                    break;
                case WellKnownFluentUiGlyphVariant.Filled:
                default:
                    fontImageSource.FontFamily = WellKnownTypography.FluentSystemIconsFilled;
                    break;
            }

            return variant;
        }
    }
}
