using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImASyncRequestExilationProcessor : ImASyncRequestProcessor
    {
        Task<OperationResult> ProcessExilation(SyncRequest syncRequest, OperationResult syncRequestProcessingResult = null);
    }
}
