using System;
using System.Collections.Generic;

namespace H.Necessaire
{
    public static class DataExtensions
    {
        public static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static long ToUnixTimestamp(this DateTime date)
        {
            TimeSpan t = (date.ToUniversalTime() - UnixEpoch);
            return (long)(t.TotalMilliseconds + 0.5);
        }

        public static DateTime UnixTimeStampToDateTime(this long unixTimeStamp)
        {
            return UnixEpoch.AddSeconds(unixTimeStamp).ToLocalTime();
        }

        public static DateTime EnsureUtc(this DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
                return dateTime;

            if (dateTime.Kind == DateTimeKind.Unspecified)
                return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

            return dateTime.ToUniversalTime();
        }

        public static float TrimToPercent(this float value)
        {
            return
                value < 0 ? 0
                : value > 100 ? 100
                : value;
        }
        public static float TrimToPercent(this int value) => TrimToPercent((float)value);
        public static float TrimToPercent(this decimal value) => TrimToPercent((float)value);
        public static float TrimToPercent(this double value) => TrimToPercent((float)value);

        public static Guid? ParseToGuidOrFallbackTo(this string rawValue, Guid? fallbackValue = null)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return fallbackValue;

            Guid parseResult;
            if (Guid.TryParse(rawValue, out parseResult))
                return parseResult;

            return fallbackValue;
        }

        public static int? ParseToIntOrFallbackTo(this string rawValue, int? fallbackValue = null)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return fallbackValue;

            int parseResult;
            if (int.TryParse(rawValue, out parseResult))
                return parseResult;

            return fallbackValue;
        }

        public static bool IsBetweenInclusive(this DateTime dateTime, DateTime? from, DateTime? to)
        {
            return
                (dateTime >= (from ?? DateTime.MinValue))
                &&
                (dateTime <= (to ?? DateTime.MaxValue));
        }

        public static T And<T>(this T data, Action<T> doThis) { doThis(data); return data; }

        public static bool IsSameOrSubclassOf(this Type typeToCheck, Type typeToCompareWith)
        {
            return
                typeToCheck == typeToCompareWith
                || typeToCompareWith.IsSubclassOf(typeToCheck);
        }

        public static IDisposableEnumerable<T> AsDisposableEnumerable<T>(this IEnumerable<T> collection)
        {
            return new Operations.Concrete.DataStream<T>(collection);
        }
    }
}
