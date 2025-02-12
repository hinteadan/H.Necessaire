namespace H.Necessaire.Runtime.MAUI.Components
{
    class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<Builders.DependencyGroup>(() => new Builders.DependencyGroup())

                .Register<ImAHMauiComponentBuilder>(() => new HMauiComponentBuilder())

                .Register<HUI.DependencyGroup>(() => new HUI.DependencyGroup())

                ;
        }
    }
}
