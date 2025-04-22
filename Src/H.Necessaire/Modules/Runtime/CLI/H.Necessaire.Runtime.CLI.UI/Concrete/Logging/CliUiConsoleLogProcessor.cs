using Spectre.Console;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.CLI.UI.Concrete.Logging
{
    internal class CliUiConsoleLogProcessor : ImALogProcessor
    {
        readonly ImALogProcessor consoleLogProcessor;
        public CliUiConsoleLogProcessor(ImALogProcessor consoleLogProcessor)
        {
            this.consoleLogProcessor = consoleLogProcessor;
        }

        public ImALogProcessor ConfigWith(LogConfig logConfig) => consoleLogProcessor.ConfigWith(logConfig);

        public LoggerPriority GetPriority() => consoleLogProcessor.GetPriority();

        public Task<bool> IsEligibleFor(LogEntry logEntry) => consoleLogProcessor.IsEligibleFor(logEntry);

        public async Task<OperationResult<LogEntry>> Process(LogEntry logEntry)
        {
            if (logEntry.Exception is null)
                return await consoleLogProcessor.Process(logEntry);

            var stackTrace = logEntry.StackTrace;
            logEntry.StackTrace = null;

            OperationResult<LogEntry> result = await consoleLogProcessor.Process(logEntry);

            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine("Exception(s):");
            AnsiConsole.WriteLine("=============");
            AnsiConsole.WriteLine();

            logEntry.Exception.CliUiPrint();

            logEntry.StackTrace = stackTrace;

            return result;
        }
    }
}
