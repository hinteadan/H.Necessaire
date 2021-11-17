using System;

namespace H.Necessaire
{
    public static class DataPrintingExtensions
    {
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
    }
}
