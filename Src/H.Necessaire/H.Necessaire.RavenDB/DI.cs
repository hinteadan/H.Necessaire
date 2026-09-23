using System;

namespace H.Necessaire.RavenDB
{
    public static class DI
    {
        public static RavenDbDocumentStore GetDefaultRavenDbDocumentStore(this ImADependencyProvider dependencyProvider)
            => dependencyProvider?.Get<RavenDbDocumentStore>();

        public static RavenDbDocumentStore NewRavenDbDocumentStore(this ImADependencyProvider dependencyProvider, Func<RuntimeConfig, ConfigNode> ravenDbConfigRootNodeProvider)
            => new RavenDbDocumentStore(ravenDbConfigRootNodeProvider).And(x => x.ReferDependencies(dependencyProvider));
        public static RavenDbDocumentStore NewRavenDbDocumentStore(this ImADependencyProvider dependencyProvider, Func<ConfigNode> ravenDbConfigRootNodeProvider)
            => new RavenDbDocumentStore(_ => ravenDbConfigRootNodeProvider?.Invoke()).And(x => x.ReferDependencies(dependencyProvider));
        public static RavenDbDocumentStore NewRavenDbDocumentStore(this ImADependencyProvider dependencyProvider, ConfigNode ravenDbConfigRootNode)
            => new RavenDbDocumentStore(_ => ravenDbConfigRootNode).And(x => x.ReferDependencies(dependencyProvider));
        public static RavenDbDocumentStore NewRavenDbDocumentStore(this ImADependencyProvider dependencyProvider)
            => new RavenDbDocumentStore().And(x => x.ReferDependencies(dependencyProvider));
    }
}
