namespace H.Necessaire.Runtime.UI.Razor
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<HUiToolkit>(() => new HUiToolkit())
                .Register<Core.Storage.DependencyGroup>(() => new Core.DependencyGroup())
                ;
        }
    }
}
