using H.Necessaire.Operations.Caching.Concrete;

namespace H.Necessaire
{
    internal class CachingDependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<CacherManager>(() => new CacherManager())
                .Register<ImACacherFactory>(() => dependencyRegistry.Get<CacherManager>())
                .Register<ImACacherRegistry>(() => dependencyRegistry.Get<CacherManager>())
                ;
        }
    }
}
