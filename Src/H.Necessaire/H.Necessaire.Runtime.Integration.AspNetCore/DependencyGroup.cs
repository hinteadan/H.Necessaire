using H.Necessaire.Runtime.Integration.DotNet;
using Microsoft.Extensions.Configuration;

namespace H.Necessaire.Runtime.Integration.AspNetCore
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
            dependencyRegistry
                .WithHNecessaireDotNetruntimeIntegration(configuration)
                .Register<UseCases.DependencyGroup>(() => new UseCases.DependencyGroup())
                ;
        }
    }
}
