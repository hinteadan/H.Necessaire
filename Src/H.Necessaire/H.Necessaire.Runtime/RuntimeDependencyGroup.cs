using H.Necessaire.Runtime.Security;

namespace H.Necessaire.Runtime
{
    public class RuntimeDependencyGroup : ImADependencyGroup
    {
        #region Construct
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<HasherFactory>(() => new HasherFactory().RegisterOrUpdateHasher(nameof(RS512Hasher), new RS512Hasher()))
                ;

            dependencyRegistry
                .Register<HNecessaireDependencyGroup>(() => new HNecessaireDependencyGroup())
                ;

            dependencyRegistry
                .Register<ExternalCommandRunner.DependencyGroup>(() => new ExternalCommandRunner.DependencyGroup())
                .Register<Resources.DependencyGroup>(() => new Resources.DependencyGroup())
                .Register<Validation.DependencyGroup>(() => new Validation.DependencyGroup())
                .Register<Security.DependencyGroup>(() => new Security.DependencyGroup())
                .Register<Sync.DependencyGroup>(() => new Sync.DependencyGroup())
                .Register<Logging.DependencyGroup>(() => new Logging.DependencyGroup())
                .Register<Analytics.DependencyGroup>(() => new Analytics.DependencyGroup())
                .Register<UseCases.DependencyGroup>(() => new UseCases.DependencyGroup())
                .Register<QdActions.DependencyGroup>(() => new QdActions.DependencyGroup())
                .Register<Daemons.DependencyGroup>(() => new Daemons.DependencyGroup())
                ;
        }
        #endregion
    }
}
