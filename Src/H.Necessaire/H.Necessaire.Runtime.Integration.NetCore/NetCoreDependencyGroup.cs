using H.Necessaire.Runtime.Integration.NetCore.Concrete;

namespace H.Necessaire.Runtime.Integration.NetCore
{
    internal class NetCoreDependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<Daemons.DependencyGroup>(() => new Daemons.DependencyGroup())
                ;

            dependencyRegistry
                .Register<NetCoreLoggerProvider>(() => new NetCoreLoggerProvider())
                ;
        }
    }
}
