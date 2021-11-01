using System;
using System.Linq;

namespace H.Necessaire
{
    public class ColorVariation
    {
        private readonly ColorInfo[] lighterShades;
        private readonly ColorInfo[] darkerShades;

        private ColorVariation() { }
        public ColorVariation(ColorInfo main, ColorInfo[] lighterShades = null, ColorInfo[] darkerShades = null)
        {
            if (main == null)
                throw new ArgumentNullException(nameof(main), "At least a main color must be given");

            Color = main;

            this.lighterShades = lighterShades ?? new ColorInfo[0];
            this.darkerShades = darkerShades ?? new ColorInfo[0];
        }

        public ColorInfo Color { get; }

        public ColorInfo Lighter(int level = 1) => GetShade(level, lighterShades);

        public ColorInfo Darker(int level = 1) => GetShade(level, darkerShades);

        private ColorInfo GetShade(int level, ColorInfo[] shades)
        {
            if (level < 1)
                return Color;

            if (!shades?.Any() ?? true)
                return Color;

            if (level >= shades.Length)
                return shades.Last();

            return shades[level - 1];
        }
    }
}
