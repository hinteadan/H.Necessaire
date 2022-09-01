using H.Necessaire.Analytics;

namespace H.Necessaire.Runtime.SqlServer.Analytics.Resources
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<ImAConsumerAnalyticsProvider>(() => new Concrete.ConsumerAnalyticsSqlServerProviderResource())
                .Register<ImANetworkAddressAnalyticsProvider>(() => new Concrete.NetworkAddressAnalyticsSqlServerProviderResource())
                ;
        }
    }
}
