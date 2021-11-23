using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public abstract class LoggerBase : ImALogger, ImADependency
    {
        #region Construct
        static readonly TimeSpan delayedProcessorsInterval = TimeSpan.FromSeconds(10);

        private readonly ConcurrentDictionary<Guid, LogEntry> logDictionary = new ConcurrentDictionary<Guid, LogEntry>();

        private string application;
        public string Application { get => application; set { application = value; logEntryDecorator = new LogEntryDecorator(Component, Application); } }

        private string component;
        public string Component { get => component; set { component = value; logEntryDecorator = new LogEntryDecorator(Component, Application); } }

        private LogEntryDecorator logEntryDecorator;
        protected LoggerBase(string component = null, string application = "H.Necessaire")
        {
            this.Application = application;
            this.Component = component;
            this.LogConfig = LogConfig.Default;
        }


        ImALogProcessor[] immediateProcessors = null;
        ImALogProcessor[] delayedProcessors = null;
        ImAPeriodicAction delayedProcessorTimer = null;
        public virtual void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            ImALogProcessorRegistry logProcessorRegistry = dependencyProvider.Get<ImALogProcessorRegistry>();
            immediateProcessors = logProcessorRegistry.GetAllKnownProcessors().Where(x => x.GetPriority().In(LoggerPriority.Immediate)).ToArray();
            delayedProcessors = logProcessorRegistry.GetAllKnownProcessors().Where(x => x.GetPriority().In(LoggerPriority.Delayed)).ToArray();

            delayedProcessorTimer?.Stop();
            delayedProcessorTimer = dependencyProvider.Get<ImAPeriodicAction>();
            delayedProcessorTimer.StartDelayed(delayedProcessorsInterval, delayedProcessorsInterval, RunDelayedProcessorsEpoch);
        }
        #endregion

        public LogConfig LogConfig { get; private set; }

        public ImALogger ConfigWith(LogConfig logConfig)
        {
            LogConfig = logConfig;
            return this;
        }

        public async Task<ImALogger> Log(LogEntry logEntry)
        {
            logEntryDecorator.DecorateLogEntry(logEntry, LogConfig.MinimumLevelForStackTrace);

            await StoreLogEntry(logEntry);

            await
                new Func<Task>(async () =>
                {
                    await
                        Task.WhenAll(
                            immediateProcessors
                            .Select(
                                processor => processor
                                .IsEligibleFor(logEntry)
                                .ContinueWith(
                                    async x => x.Result == false ? OperationResult.Win().WithPayload(logEntry) : await processor.Process(logEntry)
                                )
                            )
                            .ToArray()
                        );
                })
                .TryOrFailWithGrace(onFail: ex => { });

            return this;
        }

        public Task<ImALogger> Log(LogEntryLevel logType, string message, Exception ex = null, object payload = null, params Note[] notes)
            => Log(LogEntry.Build(logType, message, ex, payload, notes));

        public Task<ImALogger> LogCritical(string message, object payload = null, params Note[] notes)
            => Log(LogEntry.Build(LogEntryLevel.Critical, message, null, payload, notes));

        public Task<ImALogger> LogCritical(string message, Exception ex, object payload = null, params Note[] notes)
            => Log(LogEntry.Build(LogEntryLevel.Critical, message, ex, payload, notes));

        public Task<ImALogger> LogCritical(Exception ex, object payload = null, params Note[] notes)
            => Log(LogEntry.Build(LogEntryLevel.Critical, null, ex, payload, notes));

        public Task<ImALogger> LogDebug(string message, object payload = null, params Note[] notes)
            => Log(LogEntry.Build(LogEntryLevel.Debug, message, null, payload, notes));

        public Task<ImALogger> LogError(string message, object payload = null, params Note[] notes)
            => Log(LogEntry.Build(LogEntryLevel.Error, message, null, payload, notes));

        public Task<ImALogger> LogError(string message, Exception ex, object payload = null, params Note[] notes)
            => Log(LogEntry.Build(LogEntryLevel.Error, message, ex, payload, notes));

        public Task<ImALogger> LogError(Exception ex, object payload = null, params Note[] notes)
            => Log(LogEntry.Build(LogEntryLevel.Error, null, ex, payload, notes));

        public Task<ImALogger> LogInfo(string message, object payload = null, params Note[] notes)
            => Log(LogEntry.Build(LogEntryLevel.Info, message, null, payload, notes));

        public Task<ImALogger> LogTrace(string message, object payload = null, params Note[] notes)
            => Log(LogEntry.Build(LogEntryLevel.Trace, message, null, payload, notes));

        public Task<ImALogger> LogWarn(string message, object payload = null, params Note[] notes)
            => Log(LogEntry.Build(LogEntryLevel.Warn, message, null, payload, notes));

        protected virtual Task StoreLogEntry(LogEntry logEntry)
        {
            logDictionary.AddOrUpdate(logEntry.ID, logEntry, (x, y) => logEntry);
            return true.AsTask();
        }

        protected virtual Task<IDisposableEnumerable<LogEntry>> StreamAllLogEntries()
        {
            return (logDictionary.Values.ToArray().ToDisposableEnumerable()).AsTask();
        }

        protected virtual Task RemoveLogEntry(Guid logEntryID)
        {
            new Action(() =>
            {
                LogEntry logEntry;
                logDictionary.TryRemove(logEntryID, out logEntry);
            })
            .TryOrFailWithGrace();

            return true.AsTask();
        }

        protected virtual Task ClearAllLogEntires()
        {
            new Action(() =>
            {
                logDictionary.Clear();
            })
            .TryOrFailWithGrace();

            return true.AsTask();
        }

        private async Task RunDelayedProcessorsEpoch()
        {
            if (!delayedProcessors?.Any() ?? true)
            {
                await ClearAllLogEntires();
                return;
            }

            using (IDisposableEnumerable<LogEntry> logEntries = await StreamAllLogEntries())
            {
                if (!logEntries?.Any() ?? true)
                    return;

                foreach (LogEntry logEntry in logEntries)
                {
                    await
                        new Func<Task>(async () =>
                        {
                            await
                                Task.WhenAll(
                                    delayedProcessors
                                    .Select(
                                        processor => processor
                                        .IsEligibleFor(logEntry)
                                        .ContinueWith(
                                            async x => x.Result == false ? OperationResult.Win().WithPayload(logEntry) : await processor.Process(logEntry)
                                        )
                                    )
                                    .ToArray()
                                );
                        })
                        .TryOrFailWithGrace(onFail: ex => { });

                    await RemoveLogEntry(logEntry.ID);
                }
            }
        }
    }
}
