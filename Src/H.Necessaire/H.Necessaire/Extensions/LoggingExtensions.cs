using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public static class LoggingExtensions
    {
        static readonly string[] messagePartsBreaker = new string[] { ". " };
        static readonly char[] newLines = new char[] { '\r', '\n' };
        static readonly string[] ignorableLinesStartMarkers = new string[] {
            "H.Necessaire.LogEntryDecorator.",
            "H.Necessaire.DataExtensions.And[LogEntry](LogEntry data, Action`1 doThis)",
            "H.Necessaire.ExecutionUtilities.",
            "H.Necessaire.LoggerBase.",
            "H.Necessaire.ResiliencyExtensions.",
            "H.Necessaire.HSafe",
            "System.Runtime.CompilerServices.",
            "System.Threading.",
            "Foundation.NSAsyncSynchronizationContextDispatcher",
            "UIKit.UIApplication",
            "Android.App.SyncContext",
            "Java.Lang.Thread.RunnableImplementor",
            "Java.Lang.IRunnableInvoker",
            "Android.Runtime.JNINativeWrapper",
        };
        public static string[] ToLogStackTraces(this string rawStackTrace, out string[] fullStack)
        {
            fullStack = null;

            if (rawStackTrace.IsEmpty())
                return null;

            string[] lines = rawStackTrace.Split(newLines, StringSplitOptions.RemoveEmptyEntries).Select(TrimStackTraceLine).ToNonEmptyArray();

            if (lines.IsEmpty())
                return null;

            fullStack = lines;

            return
                lines
                .Where(line => !line.IsStackTraceLineIgnorable())
                .ToArrayNullIfEmpty()
                ;
        }

        public static string[] ToLogStackTraces(this string rawStackTrace) => rawStackTrace.ToLogStackTraces(out var _);

        public static string[] ToLogMessages(this string rawLogMessage)
        {
            if (rawLogMessage.IsEmpty())
                return null;

            string[] parts = rawLogMessage.Split(messagePartsBreaker, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArrayNullIfEmpty();

            return parts;
        }

        public static async Task<IDisposable> LogInfoDuration(this ImALogger log, string action, string emoji = null, Func<TimeSpan, string> endMessageBuilder = null, bool isFullyCustomized = false)
        {
            if (log is null)
                return ScopedRunner.Null;

            action = action.IfEmpty("(> Unknown Action <)");
            string actionToDisplay = isFullyCustomized ? action : string.Join(" ", "Running", action);
            string prefix = emoji.IsEmpty() ? "" : string.Join("", emoji, " ");

            await log.LogInfo($"{prefix}{actionToDisplay}");

            return new PreciseTimeMeasurement(async t => {
                string endActionToDisplay = (endMessageBuilder?.Invoke(t)).IfEmpty(string.Join(" ", "DONE", actionToDisplay, "in", t));
                string endPrefix = prefix.IsEmpty() ? "" : string.Join("", "✅", prefix);
                await log.LogInfo($"{endPrefix}{endActionToDisplay}");
            });
        }

        static string TrimStackTraceLine(this string stackTraceLine)
        {
            if (stackTraceLine.IsEmpty())
                return null;

            stackTraceLine = stackTraceLine.Trim();
            if (stackTraceLine.StartsWith("at", StringComparison.InvariantCultureIgnoreCase))
                stackTraceLine = stackTraceLine.Substring(2).Trim();

            return stackTraceLine;
        }

        static bool IsStackTraceLineIgnorable(this string stackTraceLine)
        {
            if (stackTraceLine.IsEmpty())
                return true;

            if (stackTraceLine.In(ignorableLinesStartMarkers, (line, marker) => line.StartsWith(marker, StringComparison.InvariantCultureIgnoreCase)))
                return true;

            return false;
        }
    }
}
