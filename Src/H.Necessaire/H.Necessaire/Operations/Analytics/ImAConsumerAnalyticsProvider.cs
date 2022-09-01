using System.Threading.Tasks;

namespace H.Necessaire.Analytics
{
    public interface ImAConsumerAnalyticsProvider
    {
        Task<IDisposableEnumerable<ConsumerNetworkTrace>> StreamConsumerNetworkTraces(ConsumerNetworkTraceFilter filter = null);
        Task<Page<ConsumerNetworkTrace>> GetConsumerNetworkTraces(ConsumerNetworkTracePageFilter filter = null);
    }
}
