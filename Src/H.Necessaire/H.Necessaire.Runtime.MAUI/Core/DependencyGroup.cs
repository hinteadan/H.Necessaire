namespace H.Necessaire.Runtime.MAUI.Core
{
    class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<ConsumerIdentityManager>(() => new ConsumerIdentityManager())
                .RegisterAlwaysNew<ImAUseCaseContextProvider>(() => new MauiAppUseCaseContextProvider())
                ;
        }
    }
}
