namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class CoreDependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry

                .Register<SyncDaemon>(() => new SyncDaemon())
                .Register<SyncDaemon.Worker>(() => new SyncDaemon.Worker())

                .Register<HttpClient>(() => new HttpClient())
                .Register<HNecessaireIndexedDBStorage>(() => new HNecessaireIndexedDBStorage())
                ;
        }
    }
}
