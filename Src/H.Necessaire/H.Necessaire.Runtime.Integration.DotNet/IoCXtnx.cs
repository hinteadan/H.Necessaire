using Microsoft.Extensions.Configuration;

namespace H.Necessaire.Runtime.Integration.DotNet
{
    public static class IoCXtnx
    {
        public static T WithHNecessaireDotNetRuntimeIntegration<T>(this T depsRegistry, IConfiguration configuration) where T : ImADependencyRegistry
        {
            depsRegistry
                .Register<DependencyGroup>(() => new DependencyGroup(configuration))
                ;

            return depsRegistry;
        }
    }
}
