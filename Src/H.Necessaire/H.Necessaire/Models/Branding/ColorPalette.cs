using System;
using System.Linq;

namespace H.Necessaire.Models.Branding
{
    public class ColorPalette
    {
        public static readonly ColorPalette CyanMate =
            new ColorPalette(

                new ColorVariation(
                    new ColorInfo("#476569"),
                    lighterShades: new ColorInfo[] {
                        new ColorInfo("#597478"),
                        new ColorInfo("#6c8487"),
                        new ColorInfo("#7e9396"),
                        new ColorInfo("#91a3a5"),
                        new ColorInfo("#a3b2b4"),
                        new ColorInfo("#b5c1c3"),
                        new ColorInfo("#c8d1d2"),
                        new ColorInfo("#dae0e1"),
                        new ColorInfo("#edf0f0"),
                        new ColorInfo("#ffffff"),
                    },
                    darkerShades: new ColorInfo[] {
                        new ColorInfo("#405b5f"),
                        new ColorInfo("#395154"),
                        new ColorInfo("#32474a"),
                        new ColorInfo("#2b3d3f"),
                        new ColorInfo("#243335"),
                        new ColorInfo("#1c282a"),
                        new ColorInfo("#151e1f"),
                        new ColorInfo("#0e1415"),
                        new ColorInfo("#070a0a"),
                        new ColorInfo("#000000"),
                    }
                ),

                new ColorVariation(
                    new ColorInfo("#A0D5DC"),
                    lighterShades: new ColorInfo[] {
                        new ColorInfo("#b3dde3"),
                        new ColorInfo("#c6e6ea"),
                        new ColorInfo("#d9eef1"),
                        new ColorInfo("#f6fbfc"),
                    },
                    darkerShades: new ColorInfo[] {
                        new ColorInfo("#90c0c6"),
                        new ColorInfo("#80aab0"),
                        new ColorInfo("#70959a"),
                        new ColorInfo("#608084"),
                        new ColorInfo("#506b6e"),
                        new ColorInfo("#405558"),
                        new ColorInfo("#304042"),
                        new ColorInfo("#202b2c"),
                    }
                ),

                new ColorVariation(
                    new ColorInfo("#dbf5f8"),
                    lighterShades: new ColorInfo[] {
                        new ColorInfo("#e6f8fa"),
                        new ColorInfo("#f4fcfd"),
                        new ColorInfo("#ffffff"),
                    },
                    darkerShades: new ColorInfo[] {
                        new ColorInfo("#c5dddf"),
                        new ColorInfo("#afc4c6"),
                        new ColorInfo("#99acae"),
                        new ColorInfo("#839395"),
                        new ColorInfo("#6e7b7c"),
                        new ColorInfo("#586263"),
                        new ColorInfo("#42494a"),
                    }
                )
            );

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
