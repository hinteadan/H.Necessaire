using System;
using System.Linq;

namespace H.Necessaire.Models.Branding
{
    public class ColorPalette
    {
        public static readonly ColorPalette Default =
            new ColorPalette(

                new ColorVariation(
                    new ColorInfo(42, 157, 143),
                    lighterShades: new ColorInfo[] { 
                        new ColorInfo(42, 157, 143, .65f), 
                        new ColorInfo(42, 157, 143, .25f),
                    },
                    darkerShades: new ColorInfo[] {
                        new ColorInfo(3, 20, 18),
                    }
                ),

                new ColorVariation(
                    new ColorInfo(244, 162, 97),
                    lighterShades: new ColorInfo[] {
                        new ColorInfo(244, 162, 97, .65f),
                        new ColorInfo(244, 162, 97, .25f),
                    },
                    darkerShades: new ColorInfo[] {
                        new ColorInfo(41, 27, 16),
                    }
                ),

                new ColorVariation(
                    new ColorInfo(233, 196, 106),
                    lighterShades: new ColorInfo[] {
                        new ColorInfo(233, 196, 106, .33f),
                        new ColorInfo(233, 196, 106, .13f),
                    },
                    darkerShades: new ColorInfo[] {
                        new ColorInfo(31, 25, 13),
                    }
                )
            );

        private readonly ColorVariation[] colorVariations;

        private ColorPalette() { }
        public ColorPalette(params ColorVariation[] colorVariations)
        {
            if (!colorVariations?.Any() ?? true)
                throw new ArgumentException("Must provide at least one color variation", nameof(colorVariations));

            this.colorVariations = colorVariations;
        }

        public ColorVariation Primary => colorVariations.First();
        public ColorVariation Complementary => colorVariations.Last();

        public ColorVariation PrimaryIsh(int level = 1)
        {
            if (level < 1)
                return Primary;

            if (level >= colorVariations.Length - 1)
                return colorVariations.Last();

            return colorVariations[level];
        }

        public ColorVariation ComplementaryIsh(int level = 1)
        {
            if (level < 1)
                return Complementary;

            if (level >= colorVariations.Length - 1)
                return colorVariations.First();

            return colorVariations[colorVariations.Length - 1 - level];
        }
    }
}
