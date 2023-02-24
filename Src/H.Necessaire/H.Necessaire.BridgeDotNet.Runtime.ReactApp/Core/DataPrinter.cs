using Bridge.Html5;
using System;
using System.Text;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public static class DataPrinter
    {
        static string dateTimeFormat => AppBase.Config.Get("Formatting")?.Get("DateAndTime")?.ToString() ?? "ddd, MMM dd, yyyy 'at' HH:mm 'UTC'";
        static string dateFormat => AppBase.Config.Get("Formatting")?.Get("Date")?.ToString() ?? "ddd, MMM dd, yyyy";
        static string timeFormat => AppBase.Config.Get("Formatting")?.Get("Time")?.ToString() ?? "HH:mm";
        static string monthFormat => AppBase.Config.Get("Formatting")?.Get("Month")?.ToString() ?? "yyyy MMM";
        static string dayOfWeekFormat => AppBase.Config.Get("Formatting")?.Get("DayOfWeek")?.ToString() ?? "dddd";
        static string timeStampThisYearFormat => AppBase.Config.Get("Formatting")?.Get("TimeStampThisYear")?.ToString() ?? "MMM dd 'at' HH:mm";
        static string timeStampOtherYearFormat => AppBase.Config.Get("Formatting")?.Get("TimeStampOtherYear")?.ToString() ?? "MMM dd, yyyy 'at' HH:mm";
        static string timeStampIdentifierFormat => AppBase.Config.Get("Formatting")?.Get("TimeStampIdentifier")?.ToString() ?? "yyyyMMdd_HHmmss_'UTC'";

        public static string PrintDateTimeAsOfNow(this DateTime dateTime)
        {
            TimeSpan life = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - dateTime.EnsureUtc().Ticks);
            DateTime localTime = dateTime.EnsureUtc().ToLocalTime();

            if (life < TimeSpan.FromMinutes(1))
                return "just now";
            if (life < TimeSpan.FromMinutes(5))
                return $"a few minutes ago at {localTime.ToString(timeFormat)}";
            if (life < TimeSpan.FromMinutes(59))
                return $"{(int)life.TotalMinutes} minutes ago at {localTime.ToString(timeFormat)}";
            if (life < TimeSpan.FromHours(2))
                return $"about an hour ago at {localTime.ToString(timeFormat)}";
            if (life < TimeSpan.FromHours(24))
                return $"{(int)life.TotalHours} hours ago at {localTime.ToString(timeFormat)}";
            if (life < TimeSpan.FromDays(2))
                return $"{localTime.ToString(dayOfWeekFormat)}, {(int)life.TotalDays} day ago at {localTime.ToString(timeFormat)}";
            if (life < TimeSpan.FromDays(7))
                return $"{localTime.ToString(dayOfWeekFormat)}, {(int)life.TotalDays} days ago at {localTime.ToString(timeFormat)}";

            return localTime.PrintTimeStamp();
        }
        public static string PrintDateTimeAsOfNow(this DateTime? dateTime) => dateTime is null ? "Never" : dateTime.Value.PrintDateTimeAsOfNow();

        public static string PrintDateAndTime(this DateTime dateTime)
        {
            return dateTime.EnsureUtc().ToLocalTime().ToString(dateTimeFormat);
        }
        public static string PrintDateAndTime(this DateTime? dateTime) => dateTime is null ? "Never" : dateTime.Value.PrintDateAndTime();

        public static string PrintMonth(this DateTime dateTime)
        {
            return dateTime.ToString(monthFormat);
        }
        public static string PrintMonth(this DateTime? dateTime) => dateTime is null ? "Never" : dateTime.Value.PrintMonth();

        public static string PrintDate(this DateTime dateTime)
        {
            return dateTime.EnsureUtc().ToLocalTime().ToString(dateFormat);
        }
        public static string PrintDate(this DateTime? dateTime) => dateTime is null ? "Never" : dateTime.Value.PrintDate();

        public static string PrintTime(this DateTime dateTime)
        {
            return dateTime.EnsureUtc().ToLocalTime().ToString(timeFormat);
        }
        public static string PrintTime(this DateTime? dateTime) => dateTime is null ? "Never" : dateTime.Value.PrintTime();

        public static string PrintDayOfWeek(this DateTime dateTime)
        {
            return dateTime.EnsureUtc().ToLocalTime().ToString(dayOfWeekFormat);
        }
        public static string PrintDayOfWeek(this DateTime? dateTime) => dateTime is null ? "Never" : dateTime.Value.PrintDayOfWeek();

        public static string PrintTimeStampAsIdentifier(this DateTime dateTime)
        {
            return dateTime.EnsureUtc().ToString(timeStampIdentifierFormat);
        }
        public static string PrintTimeStampAsIdentifier(this DateTime? dateTime) => dateTime is null ? "Never" : dateTime.Value.PrintTimeStampAsIdentifier();

        public static string PrintTimeStamp(this DateTime dateTime)
        {
            DateTime localTime = dateTime.EnsureUtc().ToLocalTime();
            bool isThisYear = localTime.Year == DateTime.Now.Year;
            string format = isThisYear ? timeStampThisYearFormat : timeStampOtherYearFormat;
            return localTime.ToString(format);
        }
        public static string PrintTimeStamp(this DateTime? dateTime) => dateTime is null ? "Never" : dateTime.Value.PrintTimeStamp();



        public static string Print(this PeriodOfTime happening)
        {
            if (happening.IsTimeless)
                return "Timeless";

            if (happening.IsInfinite && happening.IsSinceForever)
                return $"Since Forever - {happening.To.Value.ToLocalTime().PrintDate()} {happening.To.Value.ToLocalTime().PrintTime()}";

            if (happening.IsInfinite && happening.IsUntilForever)
                return $"{happening.From.Value.ToLocalTime().PrintDate()} {happening.From.Value.ToLocalTime().PrintTime()} - Until Forever";

            StringBuilder printer = new StringBuilder();
            printer.Append(happening.From.Value.PrintDate());

            if (happening.Duration <= TimeSpan.Zero)
                return printer.ToString();

            DateTime beginAt = happening.From.Value.ToLocalTime();
            DateTime endAt = happening.To.Value.ToLocalTime();
            bool endSameDay = beginAt.Date == endAt.Date;

            if (endSameDay)
            {
                printer.Append($" {beginAt.PrintTime()} - {endAt.PrintTime()}");
                return printer.ToString();
            }

            printer.Append($" {beginAt.PrintTime()} - {endAt.PrintDate()} {endAt.PrintTime()}");

            return printer.ToString();
        }

        public static void Download(this Blob blob, string fileName = null)
        {
            string exportUrl = JsURL.createObjectURL(blob);
            HTMLElement a = Document.CreateElement("a");
            Document.Body.AppendChild(a);
            a.Style.Display = Display.None;
            a.SetAttribute("href", exportUrl);
            a.SetAttribute("download", string.IsNullOrWhiteSpace(fileName) ? "Untitled.file" : fileName);
            a.Click();
            Window.SetTimeout(() =>
            {
                JsURL.revokeObjectURL(exportUrl);
                Document.Body.RemoveChild(a);
            }, 0);
        }
    }
}
