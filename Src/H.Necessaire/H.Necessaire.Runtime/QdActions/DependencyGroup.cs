using H.Necessaire.Runtime.QdActions.Processors;

namespace H.Necessaire.Runtime.QdActions
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<IpAddressInfoProcessor>(() => new IpAddressInfoProcessor())
                .Register<RuntimePlatformProcessor>(() => new RuntimePlatformProcessor())
                ;
        }
    }
}
