using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImASyncRequestProcessorFactory
    {
        Task<ImASyncRequestProcessor> BuildProcessorFor(SyncRequest syncRequest);

        Task<ImASyncRequestExilationProcessor> BuildExilationProcessor();
    }
}
