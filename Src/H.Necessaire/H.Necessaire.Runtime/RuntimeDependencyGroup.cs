namespace H.Necessaire.Runtime
{
    public class RuntimeDependencyGroup : ImADependencyGroup
    {
        #region Construct
        readonly bool isHttpClientCooklessCertless = false;
        public RuntimeDependencyGroup(bool isHttpClientCooklessCertless = false)
        {
            this.isHttpClientCooklessCertless = isHttpClientCooklessCertless;
        }
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<HasherFactory>(() => new HasherFactory().RegisterOrUpdateHasher(nameof(Security.RS512Hasher), new Security.RS512Hasher()))
                ;

            dependencyRegistry
                .Register<HNecessaireDependencyGroup>(() => new HNecessaireDependencyGroup())
                ;

            dependencyRegistry
                .Register<HTTP.DependencyGroup>(() => new HTTP.DependencyGroup(isHttpClientCooklessCertless))
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
