namespace H.Necessaire.Runtime.Integration.NetCore.Daemons
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<SyncRequestProcessingHostedServiceDaemon>(() => new SyncRequestProcessingHostedServiceDaemon())
                .Register<QdActionProcessingHostedServiceDaemon>(() => new QdActionProcessingHostedServiceDaemon())
                //.Register<ConsolePingDaemon>(() => new ConsolePingDaemon())
                ;
        }
    }
}
