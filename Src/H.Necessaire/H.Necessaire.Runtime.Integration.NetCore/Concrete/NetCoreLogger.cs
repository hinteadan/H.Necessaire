using Microsoft.Extensions.Logging;
using System;

namespace H.Necessaire.Runtime.Integration.NetCore.Concrete
{
    internal class NetCoreLogger : ILogger
    {
        #region Construct
        readonly ImALogger logger;
        public NetCoreLogger(ImALogger logger)
        {
            this.logger = logger;
        }
        #endregion

        public IDisposable BeginScope<TState>(TState state) => new LogStateHolder<TState>(state);

        public bool IsEnabled(LogLevel logLevel)
        {
            if (logger == null)
                return false;

            LogEntryLevel[] enabledLevels = logger.LogConfig?.ProcessEnabledLevelsFor(logger.Component) ?? LogConfig.Default.EnabledLevels;

            return logLevel.ToLogEntryLevel().In(enabledLevels);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            LogEntry logEntry = new LogEntry
            {
                Message = formatter(state, exception),
                Level = logLevel.ToLogEntryLevel(),
                Exception = exception,
                Component = logger.Component,
                Application = logger.Application,
                Notes = new Note[] {
                    new Note("Event.ID", eventId.Id.ToString()),
                    new Note("Event.Name", eventId.Name),
                },
            };

            logger.Log(logEntry).DontWait();
        }

        class LogStateHolder<TState> : IDisposable
        {
            readonly TState state;
            public LogStateHolder(TState state)
            {
                this.state = state;
            }

            public void Dispose()
            {
                if (state is IDisposable)
                {
                    new Action(() => (state as IDisposable)?.Dispose()).TryOrFailWithGrace();
                }
            }
        }
    }
}
