using H.Necessaire.Runtime.MAUI.WellKnown.FluentUI.Glyphs;

namespace H.Necessaire.Runtime.MAUI.WellKnown.FluentUI
{
    public static class WellKnownFluentUiGlyphs
    {
        static readonly IReadOnlyDictionary<string, string> defaultVariant = FluentSystemIconsFilled.GlyphsUnicodeDictionary;
        static readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> wellKnownVariants
            = new Dictionary<string, IReadOnlyDictionary<string, string>> {
                { WellKnownFluentUiGlyphVariant.Filled, FluentSystemIconsFilled.GlyphsUnicodeDictionary },
                { WellKnownFluentUiGlyphVariant.Light, FluentSystemIconsLight.GlyphsUnicodeDictionary },
                { WellKnownFluentUiGlyphVariant.Regular, FluentSystemIconsRegular.GlyphsUnicodeDictionary },
                { WellKnownFluentUiGlyphVariant.Resizable, FluentSystemIconsResizable.GlyphsUnicodeDictionary },
            }.AsReadOnly();

        /// <summary>
        /// Get the unicode value of a glyph
        /// </summary>
        /// <param name="glyphName">The firendly name displayed in HTML docs or icons index sites</param>
        /// <param name="variant">string from WellKnownFluentUiGlyphVariant</param>
        /// <returns>unicode glyph for FontImageSource</returns>
        public static string Glyph(string glyphName, string variant = null)
        {
            if (glyphName.IsEmpty())
                return glyphName;

            IReadOnlyDictionary<string, string> glyphs = variant.IsEmpty() ? defaultVariant : wellKnownVariants.GetValueOrDefault(variant, defaultVariant);

            return glyphs.GetValueOrDefault(glyphName, null);
        }
    }
}
