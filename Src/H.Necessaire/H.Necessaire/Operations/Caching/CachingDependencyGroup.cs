using H.Necessaire.Operations.Caching.Concrete;

namespace H.Necessaire
{
    internal class CachingDependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<ImACacherFactory>(() => new CacherFactory())
                ;
        }
    }
}
