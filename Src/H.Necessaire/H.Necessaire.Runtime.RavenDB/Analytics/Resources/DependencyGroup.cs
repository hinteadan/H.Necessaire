using H.Necessaire.Analytics;

namespace H.Necessaire.Runtime.RavenDB.Analytics.Resources
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<ImAConsumerAnalyticsProvider>(() => new Concrete.ConsumerAnalyticsRavenDbProviderResource())
                .Register<ImANetworkAddressAnalyticsProvider>(() => new Concrete.NetworkAddressAnalyticsRavenDbProviderResource())
                ;

        }
    }
}
