using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire
{
    internal class ConsoleLogProcessor : LogProcessorBase
    {
        const string separator = " | ";
        const ConsoleColor separatorColor = ConsoleColor.DarkGray;
        const ConsoleColor defaultColor = ConsoleColor.DarkGray;
        static readonly Dictionary<LogEntryLevel, ConsoleColor> colorPerLevel = new Dictionary<LogEntryLevel, ConsoleColor>
        {
            { LogEntryLevel.Trace, ConsoleColor.DarkBlue },
            { LogEntryLevel.Debug, ConsoleColor.DarkGray },
            { LogEntryLevel.Info, ConsoleColor.DarkCyan },
            { LogEntryLevel.Warn, ConsoleColor.DarkYellow },
            { LogEntryLevel.Error, ConsoleColor.DarkRed },
            { LogEntryLevel.Critical, ConsoleColor.Red },
        };

        public override Task<OperationResult<LogEntry>> Process(LogEntry logEntry)
        {
            if (logEntry == null || logEntry.Level == LogEntryLevel.None)
                return OperationResult.Win().WithPayload(logEntry).AsTask();

            Write(
                logEntry.HappenedAt.PrintDateAndTime()?.TupleWith(ConsoleColor.DarkGray)
                , logEntry.Message?.TupleWith(ColorFor(logEntry.Level))
                , logEntry.Method?.TupleWith(ConsoleColor.DarkGray)
                , logEntry.Component?.TupleWith(ConsoleColor.DarkGray)
            );

            if (logEntry.Payload != null)
                Write($"{Environment.NewLine}Payload: {logEntry.Payload}", ColorFor(LogEntryLevel.Debug));

            if (logEntry.Level >= LogEntryLevel.Error && !string.IsNullOrWhiteSpace(logEntry.StackTrace))
                Write($"{Environment.NewLine}Stack Trace:{Environment.NewLine}{logEntry.StackTrace}", ColorFor(LogEntryLevel.Debug));

            Console.WriteLine();

            return OperationResult.Win().WithPayload(logEntry).AsTask();
        }

        static void Write(params Tuple<string, ConsoleColor>[] values)
        {
            int index = -1;
            foreach (Tuple<string, ConsoleColor> value in values.Where(x => !string.IsNullOrWhiteSpace(x?.Item1)))
            {
                index++;
                if (index > 0) Write(separator, separatorColor);
                Write(value.Item1, value.Item2);
            }
        }

        static void Write(string value, ConsoleColor color = ConsoleColor.DarkGray) { using (color.Scope()) Console.Write(value); }

        static ConsoleColor ColorFor(LogEntryLevel level) => colorPerLevel.ContainsKey(level) ? colorPerLevel[level] : defaultColor;
    }

    class ColorScope : IDisposable
    {
        readonly ScopedRunner scopedRunner;
        public ColorScope(ConsoleColor color) => scopedRunner = new ScopedRunner(() => Console.ForegroundColor = color.And(x => { if (x == ConsoleColor.Black) Console.BackgroundColor = ConsoleColor.DarkGray; }), () => Console.ResetColor());
        public void Dispose() => scopedRunner.Dispose();
    }

    static class ConsoleLogProcessorExtensions
    {
        public static ColorScope Scope(this ConsoleColor color) => new ColorScope(color);
    }
}
