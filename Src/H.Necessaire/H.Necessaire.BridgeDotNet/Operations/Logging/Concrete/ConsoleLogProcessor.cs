using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire
{
    internal class ConsoleLogProcessor : LogProcessorBase
    {
        const string separator = " | ";

        static readonly Dictionary<LogEntryLevel, Action<object>> consoleWriters = new Dictionary<LogEntryLevel, Action<object>> {
            { LogEntryLevel.Trace, x => Bridge.Script.Call("console.debug", x) },
            { LogEntryLevel.Debug, x => Bridge.Script.Call("console.debug", x) },
            { LogEntryLevel.Info, x => Bridge.Script.Call("console.info", x) },
            { LogEntryLevel.Warn, x => Bridge.Script.Call("console.warn", x) },
            { LogEntryLevel.Error, x => Bridge.Script.Call("console.error", x) },
            { LogEntryLevel.Critical, x => Bridge.Script.Call("console.error", x) },
        };
        static readonly Action<object> defaultConsoleWriter = x => Bridge.Script.Call("console.log", x);

        public override Task<OperationResult<LogEntry>> Process(LogEntry logEntry)
        {
            if (logEntry == null || logEntry.Level == LogEntryLevel.None)
                return OperationResult.Win().WithPayload(logEntry).AsTask();

            Action<object> print = GetPrinterFor(logEntry.Level);

            string logStringToPrint
                = string.Join(separator,
                    new string[] {
                        logEntry.HappenedAt.PrintDateAndTime(),
                        logEntry.Message,
                        logEntry.Method,
                        logEntry.Component,
                    }
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                );

            print(logStringToPrint);

            if (logEntry.Payload != null)
                print(logEntry.Payload);

            return OperationResult.Win().WithPayload(logEntry).AsTask();
        }

        static Action<object> GetPrinterFor(LogEntryLevel level)
        {
            if (!consoleWriters.ContainsKey(level))
                return defaultConsoleWriter;

            return consoleWriters[level];
        }
    }
}
