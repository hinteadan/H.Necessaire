using H.Necessaire.Runtime.Integration.DotNet;
using Microsoft.Extensions.Configuration;

namespace H.Necessaire.Runtime.Integration.AspNetCore
{
    internal class DependencyGroup(IConfiguration configuration) : ImADependencyGroup
    {
        readonly IConfiguration configuration = configuration;

        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .WithHNecessaireDotNetRuntimeIntegration(configuration)
                .Register<UseCases.DependencyGroup>(() => new UseCases.DependencyGroup())
                ;
        }
    }
}
