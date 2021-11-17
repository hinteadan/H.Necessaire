using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class SyncRequestResource : HttpApiResourceBase
    {
        public async Task<OperationResult<SyncResponse>[]> Sync(SyncNode nodeToSyncWith, params SyncRequest[] syncRequests)
        {
            if (!syncRequests?.Any() ?? true)
                return new OperationResult<SyncResponse>[0];

            OperationResult<OperationResult<SyncResponse>[]> httpRequestResult =
                await SafelyRequest(() => httpClient.PostJson<OperationResult<SyncResponse>[]>($"{nodeToSyncWith.Uri}{nodeToSyncWith.SyncRequestRelativeUri}", syncRequests));

            if (!httpRequestResult.IsSuccessful)
                return syncRequests.Select(x => httpRequestResult.WithPayload(x.ToFailResponse())).ToArray();

            return httpRequestResult.Payload;
        }
    }
}
