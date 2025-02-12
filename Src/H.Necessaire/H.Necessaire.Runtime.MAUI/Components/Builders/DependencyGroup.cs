namespace H.Necessaire.Runtime.MAUI.Components.Builders
{
    class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<ImAHMauiPropertyComponentBuilder>(() => new HMauiPropertyComponentBuilder())
                ;
        }
    }
}
