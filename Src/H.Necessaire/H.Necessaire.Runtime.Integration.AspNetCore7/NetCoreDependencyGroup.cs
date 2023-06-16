using H.Necessaire.Runtime.Integration.AspNetCore7.Concrete;

namespace H.Necessaire.Runtime.Integration.AspNetCore7
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
