namespace H.Necessaire.Runtime.MAUI.Core
{
    class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<SecureStorageKeyValueStore>(() => new SecureStorageKeyValueStore())
                .Register<ConsumerIdentityManager>(() => new ConsumerIdentityManager())
                .Register<ImAConsumerUseCase>(() => dependencyRegistry.Get<ConsumerIdentityManager>())
                .Register<ImAConnectivityManager>(() => new ConnectivityManager())
                .RegisterAlwaysNew<ImAUseCaseContextProvider>(() => new MauiAppUseCaseContextProvider())
                ;
        }
    }
}
