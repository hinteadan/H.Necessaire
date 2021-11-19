using System;
using System.Threading.Tasks;

namespace H.Necessaire
{
    internal class CatchAllSyncRequestProcessor : ImASyncRequestExilationProcessor, ImADependency
    {
        static readonly IDentity processorIdentity = new InternalIdentity
        {
            ID = Guid.Parse("6A375248-9BC5-48A8-9F0F-B7952745C0E6"),
            IDTag = nameof(CatchAllSyncRequestProcessor),
            DisplayName = "Catch-All SyncRequest Processor",
        };

        ImAStorageService<string, ExiledSyncRequest> exiledSyncRequestsStorageService;
        ImAnAuditingService auditingService = null;

        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            exiledSyncRequestsStorageService = dependencyProvider.Get<ImAStorageService<string, ExiledSyncRequest>>();
            auditingService = dependencyProvider.Get<ImAnAuditingService>();
        }

        public Task<bool> IsEligibleFor(SyncRequest syncRequest) => true.AsTask();

        public Task<OperationResult> Process(SyncRequest syncRequest) => ProcessExilation(syncRequest, null);

        public async Task<OperationResult> ProcessExilation(SyncRequest syncRequest, OperationResult syncRequestProcessingResult = null)
        {
            ExiledSyncRequest exiledSyncRequest = new ExiledSyncRequest { SyncRequest = syncRequest, SyncRequestProcessingResult = syncRequestProcessingResult };

            bool dataExists = (await exiledSyncRequestsStorageService.LoadByID(exiledSyncRequest.ID))?.Payload != null;

            OperationResult saveResult = await exiledSyncRequestsStorageService?.Save(exiledSyncRequest);

            await auditingService.Append(exiledSyncRequest.ToAuditMeta<ExiledSyncRequest, string>(dataExists ? AuditActionType.Modify : AuditActionType.Create, processorIdentity), exiledSyncRequest);

            return saveResult;
        }
    }
}
