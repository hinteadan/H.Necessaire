using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImAQdActionQueueOnDemandRunner
    {
        Task<OperationResult<QdActionResult[]>> RunQdActionQueueProcessingCycle();
    }
}
