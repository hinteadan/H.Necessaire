using H.Necessaire.Runtime.MAUI.Components.Builders;

namespace H.Necessaire.Runtime.MAUI.Components
{
    class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<Builders.DependencyGroup>(() => new Builders.DependencyGroup())
                .Register<HUI.DependencyGroup>(() => new HUI.DependencyGroup())
                ;
        }
    }
}
