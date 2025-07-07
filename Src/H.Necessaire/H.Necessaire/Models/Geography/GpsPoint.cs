using System;
using System.Globalization;

namespace H.Necessaire
{
    public struct GpsPoint : IEquatable<GpsPoint>
    {
        const double meanEarthRadiusInKilometers = 6371.0088;
        const double meanEarthRadiusInMeters = meanEarthRadiusInKilometers * 1000;

        const string separator = " ";

        double latInDegress;
        public double LatInDegrees
        {
            get => latInDegress;
            set
            {
                if (value < -90) throw new ArgumentOutOfRangeException("Degrees cannot be less than 90", nameof(LatInDegrees));
                if (value > 90) throw new ArgumentOutOfRangeException("Degrees cannot be more than 90", nameof(LatInDegrees));
                latInDegress = value;
            }
        }

        double lngInDegrees;
        public double LngInDegrees
        {
            get => lngInDegrees;
            set
            {
                if (value < -180) throw new ArgumentException("Degrees cannot be less than 180", nameof(LngInDegrees));
                if (value > 180) throw new ArgumentException("Degrees cannot be more than 180", nameof(LngInDegrees));
                lngInDegrees = value;
            }
        }

        public double? AltFromSeaLevelInMeters { get; set; }

        public double DistanceIn2DTo(GpsPoint to)
        {
            return Math.Sqrt(
                Math.Pow(LngInDegrees - to.LngInDegrees, 2.0)
                +
                Math.Pow(LatInDegrees - to.LatInDegrees, 2.0)
            );
        }

        public double DistanceIn3DTo(GpsPoint to)
        {
            return Math.Sqrt(
                Math.Pow(LngInDegrees - to.LngInDegrees, 2.0)
                +
                Math.Pow(LatInDegrees - to.LatInDegrees, 2.0)
                +
                Math.Pow(AltFromSeaLevelInMeters ?? 0 - to.AltFromSeaLevelInMeters ?? 0, 2.0)
            );
        }

        public double ApproximateDistanceInMetersTo(GpsPoint to)
        {
            double latDistanceInRads = (LatInDegrees - to.LatInDegrees) * (Math.PI / 180);
            double lngDistanceInRads = (LngInDegrees - to.LngInDegrees) * (Math.PI / 180);

            double a
                = Math.Sin(latDistanceInRads / 2) * Math.Sin(latDistanceInRads / 2)
                + Math.Cos(LatInDegrees * (Math.PI / 180)) * Math.Cos(to.LatInDegrees * (Math.PI / 180))
                * Math.Sin(lngDistanceInRads / 2) * Math.Sin(lngDistanceInRads / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return meanEarthRadiusInMeters * c;
        }

        public override string ToString() => ToString(separator);

        public string ToString(string separator)
        {
            return
                string.Join(
                    separator,
                    LatInDegrees.ToString(CultureInfo.InvariantCulture),
                    LngInDegrees.ToString(CultureInfo.InvariantCulture)
                );
        }

        public bool IsSameAsIn2D(GpsPoint other)
        {
            return
                Math.Round(LatInDegrees, 12) == Math.Round(other.LatInDegrees, 12)
                && Math.Round(LngInDegrees, 12) == Math.Round(other.LngInDegrees, 12)
                ;
        }

        public static GpsPoint FromDMS(GeoDmsLatCoordinate lat, GeoDmsLngCoordinate lng, double? altFromSeaLevelInMeters = null)
        {
            return new GpsPoint
            {
                LatInDegrees = lat.ToDegrees(),
                LngInDegrees = lng.ToDegrees(),
                AltFromSeaLevelInMeters = altFromSeaLevelInMeters,
            };
        }
        public static GpsPoint FromDMS(GeoDmsCoordinates dmsCoordinates, double? altFromSeaLevelInMeters = null)
        {
            return new GpsPoint
            {
                LatInDegrees = dmsCoordinates.Lat.ToDegrees(),
                LngInDegrees = dmsCoordinates.Lng.ToDegrees(),
                AltFromSeaLevelInMeters = altFromSeaLevelInMeters,
            };
        }

        public override bool Equals(object obj)
        {
            return obj is GpsPoint point &&
                   Math.Round(LatInDegrees, 12) == Math.Round(point.LatInDegrees, 12) &&
                   Math.Round(LngInDegrees, 12) == Math.Round(point.LngInDegrees, 12) &&
                   AltFromSeaLevelInMeters == point.AltFromSeaLevelInMeters;
        }

        public bool Equals(GpsPoint other)
        {
            return
                Math.Round(LatInDegrees, 12) == Math.Round(other.LatInDegrees, 12)
                && Math.Round(LngInDegrees, 12) == Math.Round(other.LngInDegrees, 12)
                && AltFromSeaLevelInMeters == other.AltFromSeaLevelInMeters
                ;
        }

        public override int GetHashCode()
        {
            int hashCode = 1919468776;
            hashCode = hashCode * -1521134295 + LatInDegrees.GetHashCode();
            hashCode = hashCode * -1521134295 + LngInDegrees.GetHashCode();
            hashCode = hashCode * -1521134295 + AltFromSeaLevelInMeters.GetHashCode();
            return hashCode;
        }

        public static implicit operator GpsPoint((double lat, double lng, double? altFromSeaLevelInMeters) parts)
            => new GpsPoint
            {
                LatInDegrees = parts.lat,
                LngInDegrees = parts.lng,
                AltFromSeaLevelInMeters = parts.altFromSeaLevelInMeters,
            };
        public static implicit operator GpsPoint((double lat, double lng) parts)
            => new GpsPoint
            {
                LatInDegrees = parts.lat,
                LngInDegrees = parts.lng,
                AltFromSeaLevelInMeters = null,
            };
        public static implicit operator GpsPoint((GeoDmsLatCoordinate lat, GeoDmsLngCoordinate lng, double? altFromSeaLevelInMeters) parts)
            => FromDMS(parts.lat, parts.lng, parts.altFromSeaLevelInMeters);
        public static implicit operator GpsPoint((GeoDmsLatCoordinate lat, GeoDmsLngCoordinate lng) parts)
            => FromDMS(parts.lat, parts.lng);
        public static implicit operator GpsPoint((GeoDmsCoordinates geoDmsCoordinates, double? altFromSeaLevelInMeters) parts)
            => FromDMS(parts.geoDmsCoordinates, parts.altFromSeaLevelInMeters);
        public static implicit operator GpsPoint(GeoDmsCoordinates geoDmsCoordinates)
            => FromDMS(geoDmsCoordinates);

        public static bool operator ==(GpsPoint a, GpsPoint b) => a.IsSameAsIn2D(b);
        public static bool operator !=(GpsPoint a, GpsPoint b) => !a.IsSameAsIn2D(b);
    }
}
