using H.Necessaire.Runtime.MAUI.Extensions;
using H.Necessaire.Runtime.UI.Concrete;

namespace H.Necessaire.Runtime.MAUI.Components.HUI
{
    class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry

                .Register<Debugging.DependencyGroup>(() => new Debugging.DependencyGroup())

                .RegisterAlwaysNew<HMauiHUIGenericComponent<HUILoginComponent>>(() => dependencyRegistry.Get<HUILoginComponent>().ToHMauiComponent())

                ;
        }
    }
}
