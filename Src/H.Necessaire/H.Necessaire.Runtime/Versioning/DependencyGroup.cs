namespace H.Necessaire.Runtime.Versioning
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry.Register<VersionProvider>(() => new VersionProvider());
        }
    }
}
