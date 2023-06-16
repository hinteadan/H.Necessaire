namespace H.Necessaire.Runtime.Integration.AspNetCore7.Daemons
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
