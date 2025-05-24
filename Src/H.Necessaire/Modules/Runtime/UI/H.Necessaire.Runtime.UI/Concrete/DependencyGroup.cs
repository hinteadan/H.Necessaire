using H.Necessaire.Runtime.UI.Abstractions;

namespace H.Necessaire.Runtime.UI.Concrete
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry

                .Register<ImAnHUILabelProvider>(() => DefaultHUILabelProvider.Instance)

                .RegisterAlwaysNew<HUILoginComponent>(() => new HUILoginComponent("Login"))

                ;
        }
    }
}
