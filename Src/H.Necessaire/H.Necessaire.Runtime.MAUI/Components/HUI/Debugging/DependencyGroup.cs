using H.Necessaire.Runtime.MAUI.Extensions;

namespace H.Necessaire.Runtime.MAUI.Components.HUI.Debugging
{
    class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .RegisterAlwaysNew<DebuggingHUIComponent>(() => new DebuggingHUIComponent())
                .RegisterAlwaysNew<HMauiHUIGenericComponent<DebuggingHUIComponent>>(() => dependencyRegistry.Get<DebuggingHUIComponent>().ToHMauiComponent())
                ;
        }
    }
}
