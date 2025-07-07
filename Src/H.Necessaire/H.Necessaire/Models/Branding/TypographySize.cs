using System.Globalization;

namespace H.Necessaire
{
    public class TypographySize
    {
        private const int pointsPerInch = 72;
        private const int pixelsPerInch = 96;
        private const float pixelsInOnePoint = pixelsPerInch / (float)pointsPerInch;
        private const float emsInOnePoint = 1f / 12f;
        private const float percentsInOnePoint = emsInOnePoint * 100;

        private TypographySize() { }
        public TypographySize(float fontSizeInPoints = 12)
        {
            Points = fontSizeInPoints;
        }

        public float Points { get; } = 12;
        public string PointsCss => $"{Points.ToString(CultureInfo.InvariantCulture)}pt";

        public float Pixels => Points * pixelsInOnePoint;
        public string PixelsCss => $"{Pixels.ToString(CultureInfo.InvariantCulture)}px";

        public float Ems => Points * emsInOnePoint;
        public string EmsCss => $"{Ems.ToString(CultureInfo.InvariantCulture)}em";

        public float Percent => Points * percentsInOnePoint;
        public string PercentCss => $"{Percent.ToString(CultureInfo.InvariantCulture)}%";

        public TypographySize ScaleBy(float scale = 1f)
        {
            return new TypographySize(Points * scale);
        }

        public static implicit operator float(TypographySize size) => size.Points;
        public static implicit operator TypographySize(float sizeInPoints) => new TypographySize(sizeInPoints);

        public static implicit operator double(TypographySize size) => size.Points;
        public static implicit operator TypographySize(double sizeInPoints) => new TypographySize((float)sizeInPoints);
    }
}
