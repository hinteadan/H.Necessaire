using System;

namespace H.Necessaire
{
    public static class DataPrintingExtensions
    {
        public const string DateTimeFormat = "ddd, MMM dd, yyyy 'at' HH:mm:ss";
        public const string DateFormat = "ddd, MMM dd, yyyy";
        public const string TimeFormat = "HH:mm:ss";
        public const string MonthFormat = "yyyy MMM";
        public const string DayOfWeekFormat = "dddd";
        public const string TimeStampThisYearFormat = "MMM dd 'at' HH:mm:ss";
        public const string TimeStampOtherYearFormat = "MMM dd, yyyy 'at' HH:mm:ss";
        public const string TimeStampIdentifierFormat = "yyyyMMdd_HHmmss_'UTC'";
        public const string ParsableTimeStampFormat = "yyyy-MM-dd HH:mm:ss 'UTC'";

        public static string PrintPercent(this float percentValue)
        {
            return $"{Math.Round(percentValue.TrimToPercent(), 1)}%";
        }

        public static string Print(this int value)
        {
            if (value < 1000)
                return value.ToString();

            if (value < 1000 * 1000)
                return $"{(value / 1000)}k";

            if (value < 1000 * 1000 * 1000)
                return $"{(value / (1000 * 1000))}g";

            return value.ToString();
        }

        public static string PrintSize(this long sizeInBytes, uint factor = 1024, string suffix = "iB")
        {
            if (sizeInBytes < factor / 2)
                return $"{sizeInBytes} B";

            double factorAsDouble = (double)factor;
            double sizeInKiBytes = sizeInBytes / factorAsDouble;
            if (sizeInKiBytes < factor / 2)
                return $"{Math.Round(sizeInKiBytes, 2)} K{suffix}";

            double sizeInMeBytes = sizeInKiBytes / factorAsDouble;
            if (sizeInMeBytes < factor / 2)
                return $"{Math.Round(sizeInMeBytes, 2)} M{suffix}";

            double sizeInGiBytes = sizeInMeBytes / factorAsDouble;
            if (sizeInGiBytes < factor / 2)
                return $"{Math.Round(sizeInGiBytes, 2)} G{suffix}";

            double sizeInTeBytes = sizeInGiBytes / factorAsDouble;

            return $"{Math.Round(sizeInTeBytes, 2)} T{suffix}";
        }

        public static string Print(this decimal number, int numberOfDecimals = 2)
        {
            return Math.Round(number, numberOfDecimals).ToString();
        }

        public static string PrintException(this Exception ex)
        {
            return ex?.ToString();
        }

        public static string PrintTimeSpan(this TimeSpan timeSpan)
        {
            if (timeSpan.TotalSeconds < 5)
                return "a heartbeat";
            if (timeSpan.TotalMinutes < 1)
                return $"{timeSpan.Seconds} seconds";
            if (timeSpan.TotalMinutes < 2)
                return $"{timeSpan.Minutes} minute & {timeSpan.Seconds}s";
            if (timeSpan.TotalHours < 1)
                return $"{timeSpan.Minutes} minutes & {timeSpan.Seconds}s";
            if (timeSpan.TotalHours < 2)
                return $"{timeSpan.Hours} hour & {timeSpan.Minutes}m";
            if (timeSpan.TotalDays < 1)
                return $"{timeSpan.Hours} hours & {timeSpan.Minutes}m";
            if (timeSpan.TotalDays < 2)
                return $"{timeSpan.Days} day & {timeSpan.Hours}h{timeSpan.Minutes}m";

            return $"{timeSpan.Days} days & {timeSpan.Hours}h{timeSpan.Minutes}m";
        }
        public static string PrintTimeSpan(this TimeSpan? timeSpan) => timeSpan is null ? "Never" : timeSpan.Value.PrintTimeSpan();

        public static string PrintDateTimeAsOfNow(this DateTime dateTime)
        {
            TimeSpan life = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - dateTime.EnsureUtc().Ticks);
            DateTime localTime = dateTime.EnsureUtc().ToLocalTime();

            if (life < TimeSpan.FromMinutes(1))
                return "just now";
            if (life < TimeSpan.FromMinutes(5))
                return $"a few minutes ago at {localTime.ToString(TimeFormat)}";
            if (life < TimeSpan.FromMinutes(59))
                return $"{(int)life.TotalMinutes} minutes ago at {localTime.ToString(TimeFormat)}";
            if (life < TimeSpan.FromHours(2))
                return $"about an hour ago at {localTime.ToString(TimeFormat)}";
            if (life < TimeSpan.FromHours(24))
                return $"{(int)life.TotalHours} hours ago at {localTime.ToString(TimeFormat)}";
            if (life < TimeSpan.FromDays(2))
                return $"{localTime.ToString(DayOfWeekFormat)}, {(int)life.TotalDays} day ago at {localTime.ToString(TimeFormat)}";
            if (life < TimeSpan.FromDays(7))
                return $"{localTime.ToString(DayOfWeekFormat)}, {(int)life.TotalDays} days ago at {localTime.ToString(TimeFormat)}";

            return localTime.PrintTimeStamp();
        }
        public static string PrintDateTimeAsOfNow(this DateTime? dateTime) => dateTime is null ? "Never" : dateTime.Value.PrintDateTimeAsOfNow();

        public static string PrintDateAndTime(this DateTime dateTime)
        {
            return dateTime.EnsureUtc().ToLocalTime().ToString(DateTimeFormat);
        }
        public static string PrintDateAndTime(this DateTime? dateTime) => dateTime is null ? "Never" : dateTime.Value.PrintDateAndTime();

        public static string PrintMonth(this DateTime dateTime)
        {
            return dateTime.ToString(MonthFormat);
        }
        public static string PrintMonth(this DateTime? dateTime) => dateTime is null ? "Never" : dateTime.Value.PrintMonth();

        public static string PrintDate(this DateTime dateTime)
        {
            return dateTime.EnsureUtc().ToLocalTime().ToString(DateFormat);
        }
        public static string PrintDate(this DateTime? dateTime) => dateTime is null ? "Never" : dateTime.Value.PrintDate();

        public static string PrintTime(this DateTime dateTime)
        {
            return dateTime.EnsureUtc().ToLocalTime().ToString(TimeFormat);
        }
        public static string PrintTime(this DateTime? dateTime) => dateTime is null ? "Never" : dateTime.Value.PrintTime();

        public static string PrintDayOfWeek(this DateTime dateTime)
        {
            return dateTime.EnsureUtc().ToLocalTime().ToString(DayOfWeekFormat);
        }
        public static string PrintDayOfWeek(this DateTime? dateTime) => dateTime is null ? "Never" : dateTime.Value.PrintDayOfWeek();

        public static string PrintTimeStampAsIdentifier(this DateTime dateTime)
        {
            return dateTime.EnsureUtc().ToString(TimeStampIdentifierFormat);
        }
        public static string PrintTimeStampAsIdentifier(this DateTime? dateTime) => dateTime is null ? "Never" : dateTime.Value.PrintTimeStampAsIdentifier();

        public static string PrintTimeStamp(this DateTime dateTime)
        {
            DateTime localTime = dateTime.EnsureUtc().ToLocalTime();
            bool isThisYear = localTime.Year == DateTime.Now.Year;
            string format = isThisYear ? TimeStampThisYearFormat : TimeStampOtherYearFormat;
            return localTime.ToString(format);
        }
        public static string PrintTimeStamp(this DateTime? dateTime) => dateTime is null ? "Never" : dateTime.Value.PrintTimeStamp();
    }
}
