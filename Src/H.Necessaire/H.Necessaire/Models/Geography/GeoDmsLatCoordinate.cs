using System;

namespace H.Necessaire
{
    public struct GeoDmsLatCoordinate
    {
        public GeoDmsLatCoordinate(int deg, int min, double sec, GeoDmsLatDirection dir)
        {
            if (deg < 0) throw new ArgumentException("Degrees cannot be less than 0", nameof(deg));
            if (deg > 90) throw new ArgumentException("Degrees cannot be more than 90", nameof(deg));
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

        public GeoDmsLatCoordinate(int deg, int min, double sec)
            : this(Math.Abs(deg), Math.Abs(min), Math.Abs(sec), deg >= 0 ? GeoDmsLatDirection.North : GeoDmsLatDirection.South)
        { }

        public GeoDmsLatCoordinate(double decimalDegrees)
        {
            if (decimalDegrees < -90) throw new ArgumentException("Degrees cannot be less than 90", nameof(decimalDegrees));
            if (decimalDegrees > 90) throw new ArgumentException("Degrees cannot be more than 90", nameof(decimalDegrees));

            Direction = decimalDegrees >= 0 ? GeoDmsLatDirection.North : GeoDmsLatDirection.South;
            decimalDegrees = Math.Abs(decimalDegrees);

            Degrees = (int)decimalDegrees;
            decimalDegrees -= Degrees;
            Minutes = (int)(decimalDegrees * 60);
            decimalDegrees -= Minutes / 60;
            Seconds = Math.Floor(decimalDegrees * 60);
            decimalDegrees -= Seconds / 60;
            Seconds += decimalDegrees;
            //Seconds = Math.Round(Seconds, 4);
        }

        public int Degrees { get; set; }
        public int Minutes { get; set; }
        public double Seconds { get; set; }
        public GeoDmsLatDirection Direction { get; set; }
        public bool IsNorth() => Direction == GeoDmsLatDirection.North;
        public bool IsSouth() => Direction == GeoDmsLatDirection.South;

        public double ToDegrees()
            => Math.Round((Direction == GeoDmsLatDirection.North ? 1 : -1) * (Degrees + Minutes / 60d + Seconds / 3600d), 6);

        public static implicit operator GeoDmsLatCoordinate((int deg, int min, double sec, GeoDmsLatDirection dir) parts)
            => new GeoDmsLatCoordinate(parts.deg, parts.min, parts.sec, parts.dir);
        public static implicit operator GeoDmsLatCoordinate((int deg, int min, double sec) parts)
            => new GeoDmsLatCoordinate(parts.deg, parts.min, parts.sec);
        public static implicit operator GeoDmsLatCoordinate(double deg)
            => new GeoDmsLatCoordinate(deg);
        public static implicit operator GeoDmsLatCoordinate(GpsPoint gpsPoint)
            => new GeoDmsLatCoordinate(gpsPoint.LatInDegrees);
    }
}
