using System;

namespace H.Necessaire
{
    public static class DataPrintingExtensions
    {
        const string dateTimeFormat = "ddd, MMM dd, yyyy 'at' HH:mm";
        const string dateFormat = "ddd, MMM dd, yyyy";
        const string timeFormat = "HH:mm";
        const string monthFormat = "yyyy MMM";
        const string dayOfWeekFormat = "dddd";
        const string timeStampThisYearFormat = "MMM dd 'at' HH:mm";
        const string timeStampOtherYearFormat = "MMM dd, yyyy 'at' HH:mm";
        const string timeStampIdentifierFormat = "yyyyMMdd_HHmmss_'UTC'";

        public static string PrintPercent(float percentValue)
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

        public static string PrintSize(this long sizeInBytes)
        {
            float sizeInKiloBytes = sizeInBytes / 1024f;
            if (sizeInKiloBytes < 1024 / 2)
                return $"{Math.Round(sizeInKiloBytes, 2)} KiB";

            float sizeInMegaBytes = sizeInKiloBytes / 1024f;
            if (sizeInMegaBytes < 1024 / 2)
                return $"{Math.Round(sizeInMegaBytes, 2)} MiB";

            float sizeInGigaBytes = sizeInMegaBytes / 1024f;
            if (sizeInGigaBytes < 1024 / 2)
                return $"{Math.Round(sizeInGigaBytes, 2)} GiB";

            float sizeInTeraBytes = sizeInGigaBytes / 1024f;

            return $"{Math.Round(sizeInTeraBytes, 2)} TiB";
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

        public static string PrintDateTimeAsOfNow(DateTime dateTime)
        {
            DateTime now = DateTime.Now;
            DateTime localTime = dateTime.EnsureUtc().ToLocalTime();
            TimeSpan life = now - localTime;

            if (life < TimeSpan.FromMinutes(1))
                return "just now";
            if (life < TimeSpan.FromMinutes(5))
                return $"a few minutes ago at {dateTime.ToString(timeFormat)}";
            if (life < TimeSpan.FromMinutes(59))
                return $"{(int)life.TotalMinutes} minutes ago at {dateTime.ToString(timeFormat)}";
            if (life < TimeSpan.FromHours(2))
                return $"about an hour ago at {dateTime.ToString(timeFormat)}";
            if (life < TimeSpan.FromHours(24))
                return $"{(int)life.TotalHours} hours ago at {dateTime.ToString(timeFormat)}";
            if (life < TimeSpan.FromDays(2))
                return $"{dateTime.ToString(dayOfWeekFormat)}, {(int)life.TotalDays} day ago at {dateTime.ToString(timeFormat)}";
            if (life < TimeSpan.FromDays(7))
                return $"{dateTime.ToString(dayOfWeekFormat)}, {(int)life.TotalDays} days ago at {dateTime.ToString(timeFormat)}";

            return dateTime.PrintTimeStamp();
        }

        public static string PrintDateAndTime(this DateTime dateTime)
        {
            return dateTime.EnsureUtc().ToLocalTime().ToString(dateTimeFormat);
        }

        public static string PrintMonth(this DateTime dateTime)
        {
            return dateTime.ToString(monthFormat);
        }

        public static string PrintDate(this DateTime dateTime)
        {
            return dateTime.EnsureUtc().ToLocalTime().ToString(dateFormat);
        }

        public static string PrintTime(this DateTime dateTime)
        {
            return dateTime.EnsureUtc().ToLocalTime().ToString(timeFormat);
        }

        public static string PrintDayOfWeek(this DateTime dateTime)
        {
            return dateTime.EnsureUtc().ToLocalTime().ToString(dayOfWeekFormat);
        }

        public static string PrintTimeStampAsIdentifier(this DateTime dateTime)
        {
            return dateTime.EnsureUtc().ToString(timeStampIdentifierFormat);
        }

        public static string PrintTimeStamp(this DateTime dateTime)
        {
            DateTime localTime = dateTime.EnsureUtc().ToLocalTime();
            bool isThisYear = localTime.Year == DateTime.Now.Year;
            string format = isThisYear ? timeStampThisYearFormat : timeStampOtherYearFormat;
            return localTime.ToString(format);
        }
    }
}
