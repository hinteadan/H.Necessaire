using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Sync.Processors
{
    internal class LogEntryProcessor : SyncRequestProcessorBase<LogEntry>
    {
        #region Construct
        ImAStorageService<Guid, LogEntry> logEntryStorageService = null;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);

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
