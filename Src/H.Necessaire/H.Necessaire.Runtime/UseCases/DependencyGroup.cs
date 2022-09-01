using H.Necessaire.Runtime.UseCases.Concrete;

namespace H.Necessaire.Runtime.UseCases
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .RegisterAlwaysNew<ImAPingUseCase>(() => new PingUseCase())
                .RegisterAlwaysNew<ImASecurityUseCase>(() => new SecurityUseCase())
                .RegisterAlwaysNew<ImASyncUseCase>(() => new SyncUseCase())
                .RegisterAlwaysNew<ImAnAnalyticsUseCase>(() => new AnalyticsUseCase())
                .RegisterAlwaysNew<ImAVersionUseCase>(() => new VersionUseCase())
            ;
        }
    }
}
