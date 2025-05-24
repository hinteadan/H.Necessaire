namespace H.Necessaire.Runtime.Sqlite.Core
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<Resources.DependencyGroup>(() => new Resources.DependencyGroup())
                ;
        }
    }
}
