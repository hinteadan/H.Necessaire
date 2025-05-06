using System;

namespace H.Necessaire
{
    public class GeoDmsLngCoordinate
    {
        public GeoDmsLngCoordinate(double deg, double min, double sec, GeoDmsLngDirection dir)
        {
            if (deg < 0) throw new ArgumentException("Degrees cannot be less than 0", nameof(deg));
            if (deg > 180) throw new ArgumentException("Degrees cannot be more than 180", nameof(deg));
            if (min < 0) throw new ArgumentException("Minutes cannot be less than 0", nameof(min));
            if (min > 59) throw new ArgumentException("Minutes cannot be more than 59", nameof(min));
            if (sec < 0) throw new ArgumentException("Minutes cannot be less than 0", nameof(sec));
            if (sec > 59) throw new ArgumentException("Minutes cannot be more than 59", nameof(sec));
            if ((deg == 90) && (min > 0 || sec > 0)) throw new ArgumentException("Degrees+min+sec cannot be more than 90");

            Degrees = deg;
            Minutes = min;
            Seconds = sec;
            Direction = dir;
        }

        public GeoDmsLngCoordinate(double deg, double min, double sec)
            : this(Math.Abs(deg), Math.Abs(min), Math.Abs(sec), deg >= 0 ? GeoDmsLngDirection.East : GeoDmsLngDirection.West)
        { }

        public double Degrees { get; set; }
        public double Minutes { get; set; }
        public double Seconds { get; set; }
        public GeoDmsLngDirection Direction { get; set; }
    }
}
