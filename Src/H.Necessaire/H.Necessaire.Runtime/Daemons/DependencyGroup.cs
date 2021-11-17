namespace H.Necessaire.Runtime.Daemons
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry.Register<SyncRequestProcessingDaemon>(() => new SyncRequestProcessingDaemon());
        }
    }
}
