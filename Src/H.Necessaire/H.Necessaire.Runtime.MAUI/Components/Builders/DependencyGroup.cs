namespace H.Necessaire.Runtime.MAUI.Components.Builders
{
    class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<ImAHMauiHUIPropertyComponentBuilder>(() => new HMauiHUIPropertyComponentBuilder())
                .Register<ImAHMauiHUIComponentBuilder>(() => new HMauiHUIComponentBuilder())
                ;
        }
    }
}
