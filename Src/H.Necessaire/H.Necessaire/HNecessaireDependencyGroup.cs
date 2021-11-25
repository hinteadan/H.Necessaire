using H.Necessaire.Operations.Versioning;

namespace H.Necessaire
{
    public class HNecessaireDependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .RegisterAlwaysNew<ImAPeriodicAction>(() => ConcreteFactory.BuildNewPeriodicAction())
                ;

            dependencyRegistry
                .Register<SyncDependencyGroup>(() => new SyncDependencyGroup())
                .Register<LoggingDependencyGroup>(() => new LoggingDependencyGroup())
                .Register<VersioningDependencyGroup>(() => new VersioningDependencyGroup())
                .Register<Operations.QdAction.DependenctGroup>(() => new Operations.QdAction.DependenctGroup())
                ;
        }
    }
}
