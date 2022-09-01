using System.Threading.Tasks;

namespace H.Necessaire.Analytics
{
    public interface ImANetworkAddressAnalyticsProvider
    {
        Task<IDisposableEnumerable<IpAddressNetworkTrace>> StreamIpAddressNetworkTraces(IpAddressNetworkTraceFilter filter = null);
        Task<Page<IpAddressNetworkTrace>> GetIpAddressNetworkTraces(IpAddressNetworkTracePageFilter filter = null);
    }
}
