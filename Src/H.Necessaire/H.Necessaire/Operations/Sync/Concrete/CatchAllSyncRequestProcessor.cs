using System.Threading.Tasks;

namespace H.Necessaire
{
    internal class CatchAllSyncRequestProcessor : ImASyncRequestExilationProcessor, ImADependency
    {
        ImAStorageService<string, ExiledSyncRequest> exiledSyncRequestsStorageService;

        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            exiledSyncRequestsStorageService = dependencyProvider.Get<ImAStorageService<string, ExiledSyncRequest>>();
        }

        public Task<bool> IsEligibleFor(SyncRequest syncRequest) => true.AsTask();

        public Task<OperationResult> Process(SyncRequest syncRequest) => ProcessExilation(syncRequest, null);

        public async Task<OperationResult> ProcessExilation(SyncRequest syncRequest, OperationResult syncRequestProcessingResult = null)
        {
            ExiledSyncRequest exiledSyncRequest = new ExiledSyncRequest { SyncRequest = syncRequest, SyncRequestProcessingResult = syncRequestProcessingResult };

            OperationResult saveResult = await exiledSyncRequestsStorageService?.Save(exiledSyncRequest);

            return saveResult;
        }
    }
}
