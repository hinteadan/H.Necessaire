using H.Necessaire.Analytics;
using H.Necessaire.Runtime.Analytics.Managers;
using System.IO;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.UseCases.Concrete
{
    class AnalyticsUseCase : UseCaseBase, ImAnAnalyticsUseCase
    {
        #region Construct
        ImAnAnalyticsManager analyticsManager;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            analyticsManager = dependencyProvider.Get<ImAnAnalyticsManager>();
        }
        #endregion

        public async Task<Page<ConsumerNetworkTrace>> GetConsumerNetworkTraces(ConsumerNetworkTracePageFilter filter = null)
        {
            (await EnsureAuthenticationAndPermissions(WellKnownPermissionClaim.AnalyticsRead)).ThrowOnFail();

            return await analyticsManager.GetConsumerNetworkTraces(filter);
        }

        public async Task<Page<IpAddressNetworkTrace>> GetIpAddressNetworkTraces(IpAddressNetworkTracePageFilter filter = null)
        {
            (await EnsureAuthenticationAndPermissions(WellKnownPermissionClaim.AnalyticsRead)).ThrowOnFail();

            return await analyticsManager.GetIpAddressNetworkTraces(filter);
        }

        public async Task<Stream> StreamConsumerNetworkTraces(ConsumerNetworkTraceFilter filter = null)
        {
            (await EnsureAuthenticationAndPermissions(WellKnownPermissionClaim.AnalyticsRead)).ThrowOnFail();

            using (IDisposableEnumerable<ConsumerNetworkTrace> traces = await analyticsManager.StreamConsumerNetworkTraces(filter))
            {
                return await traces.ToJsonUTF8Stream();
            }
        }

        public async Task<Stream> StreamIpAddressNetworkTraces(IpAddressNetworkTraceFilter filter = null)
        {
            (await EnsureAuthenticationAndPermissions(WellKnownPermissionClaim.AnalyticsRead)).ThrowOnFail();

            using (IDisposableEnumerable<IpAddressNetworkTrace> traces = await analyticsManager.StreamIpAddressNetworkTraces(filter))
            {
                return await traces.ToJsonUTF8Stream();
            }
        }
    }
}
