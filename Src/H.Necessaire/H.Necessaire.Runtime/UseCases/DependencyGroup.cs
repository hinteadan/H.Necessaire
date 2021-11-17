namespace H.Necessaire.Runtime.UseCases
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry.RegisterAlwaysNew<ImAPingUseCase>(() => new PingUseCase());
            dependencyRegistry.RegisterAlwaysNew<ImASecurityUseCase>(() => new SecurityUseCase());
            dependencyRegistry.RegisterAlwaysNew<ImASyncUseCase>(() => new SyncUseCase());
        }
    }
}
