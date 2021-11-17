using H.Necessaire.Operations.Sync.Concrete;

namespace H.Necessaire
{
    public class SyncDependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry.Register<ImASyncableBrowser>(() => new SyncableBrowser());
            dependencyRegistry.Register<CatchAllSyncRequestProcessor>(() => new CatchAllSyncRequestProcessor());
            dependencyRegistry.Register<ImASyncRequestProcessorFactory>(() => new SyncRequestProcessorFactory());
        }
    }
}
