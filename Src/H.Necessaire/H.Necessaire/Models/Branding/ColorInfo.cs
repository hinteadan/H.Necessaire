using System;

namespace H.Necessaire
{
    public class ColorInfo
    {
        private ColorInfo() { }
        public ColorInfo(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                throw new ArgumentNullException(nameof(hex), "Must provide a color");

            hex = hex?.Replace("#", string.Empty).ToUpper();

            if (hex.Length != 6)
                throw new InvalidOperationException("Color hex must be 6 chars long");

            Hex = $"#{hex}";

            Red = Convert.ToByte(hex.Substring(0, 2), 16);
            Green = Convert.ToByte(hex.Substring(2, 2), 16);
            Blue = Convert.ToByte(hex.Substring(4, 2), 16);
        }

        public ColorInfo(byte red, byte green, byte blue, float opacity = 1f)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Opacity = opacity;
            Hex = BitConverter.ToString(new byte[] { red, green, blue }).Replace("-", string.Empty);
        }

        public byte Red { get; } = 0;
        public byte Green { get; } = 0;
        public byte Blue { get; } = 0;
        public float Opacity { get; } = 1f;
        public string Hex { get; } = "#000000";
        public string ToCssRGBA() => $"rgba({Red},{Green},{Blue},{Opacity})";
    }
}
