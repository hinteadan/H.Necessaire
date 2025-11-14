namespace H.Necessaire.Runtime.UI.Razor.Core.Managers
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<ConsumerManager>(() => new ConsumerManager())
                ;
        }
    }
}
