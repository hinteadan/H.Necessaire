using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImASyncRequestProcessor
    {
        Task<bool> IsEligibleFor(SyncRequest syncRequest);
        Task<OperationResult> Process(SyncRequest syncRequest);
    }
}
