using System;

namespace H.Necessaire
{
    public struct GeoDmsLngCoordinate
    {
        public GeoDmsLngCoordinate(int deg, int min, double sec, GeoDmsLngDirection dir)
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

        public GeoDmsLngCoordinate(int deg, int min, double sec)
            : this(Math.Abs(deg), Math.Abs(min), Math.Abs(sec), deg >= 0 ? GeoDmsLngDirection.East : GeoDmsLngDirection.West)
        { }

        public GeoDmsLngCoordinate(double decimalDegrees)
        {
            if (decimalDegrees < -180) throw new ArgumentException("Degrees cannot be less than 180", nameof(decimalDegrees));
            if (decimalDegrees > 180) throw new ArgumentException("Degrees cannot be more than 180", nameof(decimalDegrees));

            Direction = decimalDegrees >= 0 ? GeoDmsLngDirection.East : GeoDmsLngDirection.West;

            decimalDegrees.ToDMS(out var deg, out var min, out var sec, isUnsigned: true);

            Degrees = deg;
            Minutes = min;
            Seconds= sec;
        }

        public int Degrees { get; set; }
        public int Minutes { get; set; }
        public double Seconds { get; set; }
        public GeoDmsLngDirection Direction { get; set; }
        public bool IsEast() => Direction == GeoDmsLngDirection.East;
        public bool IsWest() => Direction == GeoDmsLngDirection.West;
        public bool IsPositive() => IsEast();
        public bool IsNegative() => IsWest();

        public double ToDegrees()
        {
            double result = Degrees.ToDegrees(Minutes, Seconds);
            return IsPositive() ? result : -result;
        }

        public static implicit operator GeoDmsLngCoordinate((int deg, int min, double sec, GeoDmsLngDirection dir) parts)
            => new GeoDmsLngCoordinate(parts.deg, parts.min, parts.sec, parts.dir);
        public static implicit operator GeoDmsLngCoordinate((int deg, int min, double sec) parts)
            => new GeoDmsLngCoordinate(parts.deg, parts.min, parts.sec);
        public static implicit operator GeoDmsLngCoordinate(double deg)
            => new GeoDmsLngCoordinate(deg);
        public static implicit operator GeoDmsLngCoordinate(GpsPoint gpsPoint)
            => new GeoDmsLngCoordinate(gpsPoint.LngInDegrees);
    }
}
