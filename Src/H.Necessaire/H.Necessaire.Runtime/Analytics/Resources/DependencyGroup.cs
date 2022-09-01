using H.Necessaire.Analytics;

namespace H.Necessaire.Runtime.Analytics.Resources
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<ImAConsumerAnalyticsProvider>(() => new Concrete.ConsumerAnalyticsFileSystemProviderResource())
                .Register<ImANetworkAddressAnalyticsProvider>(() => new Concrete.NetworkAddressAnalyticsFileSystemProviderResource())
                ;
        }
    }
}
