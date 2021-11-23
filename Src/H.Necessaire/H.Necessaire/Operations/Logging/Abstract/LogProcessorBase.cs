using System.Threading.Tasks;

namespace H.Necessaire
{
    public abstract class LogProcessorBase : ImALogProcessor
    {
        protected LogConfig logConfig = LogConfig.Default;

        protected virtual LogConfig GetLogConfig() => logConfig;

        public virtual LoggerPriority GetPriority() => LoggerPriority.Immediate;

        public virtual ImALogProcessor ConfigWith(LogConfig logConfig)
        {
            this.logConfig = logConfig ?? this.logConfig;
            return this;
        }

        public virtual Task<bool> IsEligibleFor(LogEntry logEntry)
        {
            if (logEntry == null)
                return false.AsTask();

            LogEntryLevel[] enabledLevels = logConfig?.ProcessEnabledLevelsFor(logEntry?.Component) ?? LogConfig.Default.EnabledLevels;

            return
                logEntry.Level.In(enabledLevels).AsTask();
        }

        public abstract Task<OperationResult<LogEntry>> Process(LogEntry logEntry);
    }
}
