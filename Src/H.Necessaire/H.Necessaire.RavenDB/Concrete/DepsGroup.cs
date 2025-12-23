namespace H.Necessaire.RavenDB.Concrete
{
    internal class DepsGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<ImADistributedLocker>(() => new RavenDbDistributedLocker())
                ;
        }
    }
}
