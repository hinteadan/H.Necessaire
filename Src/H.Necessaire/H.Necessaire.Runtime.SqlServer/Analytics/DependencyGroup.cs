namespace H.Necessaire.Runtime.SqlServer.Analytics
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
