using H.Necessaire.Runtime.Integration.NetCore.Concrete;

namespace H.Necessaire.Runtime.Integration.NetCore
{
    internal class NetCoreDependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<HNecessaireDependencyGroup>(() => new HNecessaireDependencyGroup())
                .Register<SyncRequestProcessingHostedServiceDaemon>(() => new SyncRequestProcessingHostedServiceDaemon())
                //.Register<ConsolePingDaemon>(() => new ConsolePingDaemon())
                ;

            dependencyRegistry
                .Register<NetCoreLoggerProvider>(() => new NetCoreLoggerProvider())
                ;
        }
    }
}
