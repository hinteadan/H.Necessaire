using H.Necessaire.Operations.Versioning.Concrete;

namespace H.Necessaire.Operations.Versioning
{
    internal class VersioningDependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<ImAVersionProvider>(() => new EmbeddedResourceVersionProvider())
                ;
        }
    }
}
