using H.Necessaire.Analytics;
using H.Necessaire.Runtime.Google.FirestoreDB.Analytics.Resources;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Analytics
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<ImAConsumerAnalyticsProvider>(() => new ConsumerAnalyticsGoogleFirestoreDbProviderResource())
                .Register<ImANetworkAddressAnalyticsProvider>(() => new NetworkAddressAnalyticsGoogleFirestoreDbProviderResource())
                ;
        }
    }
}
