using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.MAUI.Extensions
{
    public static class ColorExtensions
    {
        static readonly DataNormalizer opacityNormalizer = new DataNormalizer(fromInterval: new NumberInterval(0, 1), toInterval: new NumberInterval(byte.MinValue, byte.MaxValue));
        public static Color ToMaui(this ColorInfo colorInfo)
        {
            if (colorInfo is null)
                return Colors.Transparent;

            if (colorInfo.Opacity <= 0)
                return Colors.Transparent;

            if (colorInfo.Opacity >= 1)
                return Color.FromRgb(colorInfo.Red, colorInfo.Green, colorInfo.Blue);

            byte opacityAsByte = (byte)Math.Round(opacityNormalizer.Do((decimal)colorInfo.Opacity));

            return Color.FromRgba(colorInfo.Red, colorInfo.Green, colorInfo.Blue, opacityAsByte);
        }
    }
}
