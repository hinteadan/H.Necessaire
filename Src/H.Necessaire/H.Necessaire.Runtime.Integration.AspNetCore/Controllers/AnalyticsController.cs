using H.Necessaire.Analytics;
using H.Necessaire.Runtime.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace H.Necessaire.Runtime.Integration.AspNetCore.Controllers
{
    [Route(HAspNetCoreConstants.ApiControllerBaseRoute)]
    [ApiController]
    public class AnalyticsController : ControllerBase, ImAnAnalyticsUseCase
    {
        #region Construct
        readonly ImAnAnalyticsUseCase useCase;
        public AnalyticsController
            (
            ImAnAnalyticsUseCase useCase
            )
        {
            this.useCase = useCase;
        }
        #endregion

        [Route(nameof(GetConsumerNetworkTraces)), HttpPost]
        public Task<Page<ConsumerNetworkTrace>> GetConsumerNetworkTraces(ConsumerNetworkTracePageFilter filter = null)
            => useCase.GetConsumerNetworkTraces(filter);

        [Route(nameof(GetIpAddressNetworkTraces)), HttpPost]
        public Task<Page<IpAddressNetworkTrace>> GetIpAddressNetworkTraces(IpAddressNetworkTracePageFilter filter = null)
            => useCase.GetIpAddressNetworkTraces(filter);

        [Route(nameof(StreamConsumerNetworkTraces)), HttpPost]
        public Task<Stream> StreamConsumerNetworkTraces(ConsumerNetworkTraceFilter filter = null)
        {
            Response.ContentType = "application/json; charset=utf-8";
            return useCase.StreamConsumerNetworkTraces(filter);
        }

        [Route(nameof(StreamIpAddressNetworkTraces)), HttpPost]
        public Task<Stream> StreamIpAddressNetworkTraces(IpAddressNetworkTraceFilter filter = null)
        {
            Response.ContentType = "application/json; charset=utf-8";
            return useCase.StreamIpAddressNetworkTraces(filter);
        }
    }
}
