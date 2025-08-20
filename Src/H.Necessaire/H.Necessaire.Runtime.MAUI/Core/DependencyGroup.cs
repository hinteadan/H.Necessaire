using H.Necessaire.Operations;
using H.Necessaire.UI;

namespace H.Necessaire.Runtime.MAUI.Core
{
    class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<SecureStorageKeyValueStore>(() => new SecureStorageKeyValueStore())
                .Register<PreferencesKeyValueStore>(() => new PreferencesKeyValueStore())
                .Register<ConsumerIdentityManager>(() => new ConsumerIdentityManager())
                .Register<ImAConsumerUseCase>(() => dependencyRegistry.Get<ConsumerIdentityManager>())
                .RegisterAlwaysNew<ImAHealthChecker>(() => new NetMauiHealthChecker())
                .RegisterAlwaysNew<ImAUseCaseContextProvider>(() => new MauiAppUseCaseContextProvider())
                .Register<ImAConnectivityInfoProvider>(() => new NetMauiConnectivityInfoProvider())
                .RegisterAlwaysNew<ImAModalController>(() => new HMauiModalController())
                ;
        }
    }
}
