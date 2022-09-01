using H.Necessaire.Analytics;
using System.IO;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.UseCases
{
    public interface ImAnAnalyticsUseCase : ImAUseCase
    {
        Task<Page<ConsumerNetworkTrace>> GetConsumerNetworkTraces(ConsumerNetworkTracePageFilter filter = null);
        Task<Page<IpAddressNetworkTrace>> GetIpAddressNetworkTraces(IpAddressNetworkTracePageFilter filter = null);

        Task<Stream> StreamConsumerNetworkTraces(ConsumerNetworkTraceFilter filter = null);
        Task<Stream> StreamIpAddressNetworkTraces(IpAddressNetworkTraceFilter filter = null);
    }
}
