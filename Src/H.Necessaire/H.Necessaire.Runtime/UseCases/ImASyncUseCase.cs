using System.Threading.Tasks;

namespace H.Necessaire.Runtime
{
    public interface ImASyncUseCase : ImAUseCase
    {
        Task<OperationResult<SyncResponse>[]> Sync(params SyncRequest[] syncRequests);
    }
}
