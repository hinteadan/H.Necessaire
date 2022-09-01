using H.Necessaire.Analytics;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Analytics.Managers.Concrete
{
    internal class AnalyticsManager : ImADependency, ImAnAnalyticsManager
    {
        #region Construct
        ImALogger logger;
        ImAConsumerAnalyticsProvider consumerAnalyticsProvider;
        ImANetworkAddressAnalyticsProvider networkAddressAnalyticsProvider;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            logger = dependencyProvider.GetLogger<AnalyticsManager>();
            consumerAnalyticsProvider = dependencyProvider.Get<ImAConsumerAnalyticsProvider>();
            networkAddressAnalyticsProvider = dependencyProvider.Get<ImANetworkAddressAnalyticsProvider>();
        }
        #endregion

        public async Task<Page<ConsumerNetworkTrace>> GetConsumerNetworkTraces(ConsumerNetworkTracePageFilter filter = null)
        {
            OperationResult<Page<ConsumerNetworkTrace>> result = OperationResult.Fail("Not yet started").WithoutPayload<Page<ConsumerNetworkTrace>>();

            await
                new Func<Task>(async () =>
                {
                    result
                        = (await consumerAnalyticsProvider.GetConsumerNetworkTraces(filter))
                        .ToWinResult();
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        string reason = "An error occured while trying to Get Consumer Network Traces Page";
                        await logger.LogError(reason, ex);
                        result = OperationResult.Fail(ex, reason).WithoutPayload<Page<ConsumerNetworkTrace>>();
                    }
                );

            return result.ThrowOnFailOrReturn();
        }

        public async Task<Page<IpAddressNetworkTrace>> GetIpAddressNetworkTraces(IpAddressNetworkTracePageFilter filter = null)
        {
            OperationResult<Page<IpAddressNetworkTrace>> result = OperationResult.Fail("Not yet started").WithoutPayload<Page<IpAddressNetworkTrace>>();

            await
                new Func<Task>(async () =>
                {
                    result
                        = (await networkAddressAnalyticsProvider.GetIpAddressNetworkTraces(filter))
                        .ToWinResult();
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        string reason = "An error occured while trying to Get Ip Address Network Traces Page";
                        await logger.LogError(reason, ex);
                        result = OperationResult.Fail(ex, reason).WithoutPayload<Page<IpAddressNetworkTrace>>();
                    }
                );

            return result.ThrowOnFailOrReturn();
        }

        public async Task<IDisposableEnumerable<ConsumerNetworkTrace>> StreamConsumerNetworkTraces(ConsumerNetworkTraceFilter filter = null)
        {
            OperationResult<IDisposableEnumerable<ConsumerNetworkTrace>> result = OperationResult.Fail("Not yet started").WithoutPayload<IDisposableEnumerable<ConsumerNetworkTrace>>();

            await
                new Func<Task>(async () =>
                {
                    result
                        = (await consumerAnalyticsProvider.StreamConsumerNetworkTraces(filter))
                        .ToWinResult();
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        string reason = "An error occured while trying to Stream Consumer Network Traces";
                        await logger.LogError(reason, ex);
                        result = OperationResult.Fail(ex, reason).WithoutPayload<IDisposableEnumerable<ConsumerNetworkTrace>>();
                    }
                );

            return result.ThrowOnFailOrReturn();
        }

        public async Task<IDisposableEnumerable<IpAddressNetworkTrace>> StreamIpAddressNetworkTraces(IpAddressNetworkTraceFilter filter = null)
        {
            OperationResult<IDisposableEnumerable<IpAddressNetworkTrace>> result = OperationResult.Fail("Not yet started").WithoutPayload<IDisposableEnumerable<IpAddressNetworkTrace>>();

            await
                new Func<Task>(async () =>
                {
                    result
                        = (await networkAddressAnalyticsProvider.StreamIpAddressNetworkTraces(filter))
                        .ToWinResult();
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        string reason = "An error occured while trying to Stream Ip Address Network Traces";
                        await logger.LogError(reason, ex);
                        result = OperationResult.Fail(ex, reason).WithoutPayload<IDisposableEnumerable<IpAddressNetworkTrace>>();
                    }
                );

            return result.ThrowOnFailOrReturn();
        }
    }
}
