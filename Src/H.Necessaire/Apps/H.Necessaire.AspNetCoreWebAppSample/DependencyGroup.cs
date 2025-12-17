using H.Necessaire.Operations.Versioning.Concrete;

namespace H.Necessaire.AspNetCoreWebAppSample
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<ImAVersionProvider>(() => new EmbeddedResourceVersionProvider(typeof(DependencyGroup).Assembly))
                ;
        }
    }
}
