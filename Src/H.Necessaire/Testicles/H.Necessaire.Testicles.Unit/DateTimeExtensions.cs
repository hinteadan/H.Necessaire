using System;

namespace H.Necessaire.Testicles.Unit
{
    internal static class DateTimeExtensions
    {
        public static DateTime WithoutMicroseconds(this DateTime dateTime)
        {
            return
                new DateTime(
                    year: dateTime.Year,
                    month: dateTime.Month,
                    day: dateTime.Day,
                    hour: dateTime.Hour,
                    minute: dateTime.Minute,
                    second: dateTime.Second,
                    millisecond: dateTime.Millisecond,
                    microsecond: 0,
                    kind: dateTime.Kind
                );
        }
    }
}
