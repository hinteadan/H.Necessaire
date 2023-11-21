using H.Necessaire.BridgeDotNet.Runtime.ReactApp.Core.Storage;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class CoreDependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry

                .Register<SyncDaemon>(() => new SyncDaemon())
                .Register<SyncDaemon.Worker>(() => new SyncDaemon.Worker())

                .Register<SecurityContextUpdateDaemon>(() => new SecurityContextUpdateDaemon())
                .Register<SecurityContextUpdateDaemon.Worker>(() => new SecurityContextUpdateDaemon.Worker())

                .Register<HttpClient>(() => new HttpClient())

                .Register<HNecessaireIndexedDBStorage>(() => new HNecessaireIndexedDBStorage())
                .Register<AppStateIndexedDBStorage>(() => new AppStateIndexedDBStorage())
                ;
        }
    }
}
