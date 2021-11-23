using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Sync.Processors
{
    internal class LogEntryProcessor : SyncRequestProcessorBase<LogEntry>, ImADependency
    {
        #region Construct
        ImAStorageService<Guid, LogEntry> logEntryStorageService = null;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            logEntryStorageService = dependencyProvider.Get<ImAStorageService<Guid, LogEntry>>();
        }
        #endregion

        protected override async Task<OperationResult> ProcessPayload(LogEntry payload, SyncRequest syncRequest)
        {
            return
                await logEntryStorageService.Save(payload);
        }
    }
}
