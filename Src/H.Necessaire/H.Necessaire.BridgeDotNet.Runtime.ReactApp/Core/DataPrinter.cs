using Bridge.Html5;
using System;
using System.Text;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public static class DataPrinter
    {
        static readonly string dateTimeDefaultFormat = AppBase.Config.Get("Formatting")?.Get("DateAndTime")?.ToString() ?? "ddd, MMM dd, yyyy 'at' HH:mm 'UTC'";
        static readonly string dateDefaultFormat = AppBase.Config.Get("Formatting")?.Get("Date")?.ToString() ?? "ddd, MMM dd, yyyy";
        static readonly string timeDefaultFormat = AppBase.Config.Get("Formatting")?.Get("Time")?.ToString() ?? "HH:mm";
        static readonly string monthDefaultFormat = AppBase.Config.Get("Formatting")?.Get("Month")?.ToString() ?? "yyyy MMM";

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

        public static string Print(this DateTime dateTime)
        {
            return dateTime.EnsureUtc().ToString(dateTimeDefaultFormat);
        }

        public static string PrintMonth(this DateTime dateTime)
        {
            return dateTime.ToString(monthDefaultFormat);
        }

        public static string PrintLocalDate(this DateTime dateTime)
        {
            return dateTime.EnsureUtc().ToLocalTime().ToString(dateDefaultFormat);
        }

        public static string PrintLocalTime(this DateTime dateTime)
        {
            return dateTime.EnsureUtc().ToLocalTime().ToString(timeDefaultFormat);
        }

        public static string Print(this decimal number, int numberOfDecimals = 2)
        {
            return Math.Round(number, numberOfDecimals).ToString();
        }

        public static string Print(this PeriodOfTime happening)
        {
            StringBuilder printer = new StringBuilder(happening.From.PrintLocalDate());

            if (happening.Duration <= TimeSpan.Zero)
                return printer.ToString();

            DateTime beginAt = happening.From.ToLocalTime();
            DateTime endAt = happening.To.ToLocalTime();
            bool endSameDay = beginAt.Date == endAt.Date;

            if (endSameDay)
            {
                printer.Append($" {beginAt.PrintLocalTime()} - {endAt.PrintLocalTime()}");
                return printer.ToString();
            }

            printer.Append($" {beginAt.PrintLocalTime()} - {endAt.PrintLocalDate()} {endAt.PrintLocalTime()}");

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
