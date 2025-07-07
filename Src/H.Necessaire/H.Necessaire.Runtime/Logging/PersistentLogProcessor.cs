using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Logging
{
    internal class PersistentLogProcessor : LogProcessorBase, ImADependency
    {
        static readonly LogConfig defaultConfig = new LogConfig { EnabledLevels = LogConfig.LevelsHigherOrEqualTo(LogEntryLevel.Warn, includeNone: false) };

        public PersistentLogProcessor()
        {
            logConfig = defaultConfig;
        }

        ImAStorageService<Guid, LogEntry> storageService;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            storageService = dependencyProvider.Get<ImAStorageService<Guid, LogEntry>>();
        }

        public override LoggerPriority GetPriority() => LoggerPriority.Delayed;

        public override async Task<OperationResult<LogEntry>> Process(LogEntry logEntry)
        {
            if (storageService is null)
                return logEntry;

            return (await storageService.Save(logEntry)).WithPayload(logEntry);
        }
    }
}
