using H.Necessaire.Runtime.Integration.DotNet.Concrete;
using Microsoft.Extensions.Configuration;

namespace H.Necessaire.Runtime.Integration.DotNet
{
    internal class DependencyGroup : ImADependencyGroup
    {
        readonly IConfiguration configuration;
        public DependencyGroup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            bool isRuntimeDependencyGroupAlreadyRegistered = dependencyRegistry.Get<RuntimeDependencyGroup>() != null;

            RuntimeConfig runtimeConfig = dependencyRegistry.GetRuntimeConfig();
            dependencyRegistry
                .AndIf(!isRuntimeDependencyGroupAlreadyRegistered, x => x.Register<RuntimeDependencyGroup>(() => new RuntimeDependencyGroup()))
                .Register<NetCoreLoggerProvider>(() => new NetCoreLoggerProvider())
                .Register<ImAUseCaseContextProvider>(() => new Concrete.DotNetEnvironmentUseCaseContextProvider())
                .Register<ImAConfigProvider>(() => new Concrete.NetCoreConfigProvider(runtimeConfig, configuration))
                ;
        }
    }
}
