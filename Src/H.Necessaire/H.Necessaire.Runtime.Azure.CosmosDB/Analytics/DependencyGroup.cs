using H.Necessaire.Analytics;
using H.Necessaire.Runtime.Azure.CosmosDB.Analytics.Resources;

namespace H.Necessaire.Runtime.Azure.CosmosDB.Analytics
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<ImAConsumerAnalyticsProvider>(() => new ConsumerAnalyticsAzureCosmosDbProviderResource())
                .Register<ImANetworkAddressAnalyticsProvider>(() => new NetworkAddressAnalyticsAzureCosmosDbProviderResource())
                ;
        }
    }
}
