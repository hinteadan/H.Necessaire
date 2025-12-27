namespace H.Necessaire.RavenDB
{
    public abstract class RavenDbDependencyGroup : ImADependencyGroup
    {
        #region Construct
        public virtual void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<RavenDbDocumentStore>(() => new RavenDbDocumentStore())
                .Register<Concrete.DepsGroup>(() => new Concrete.DepsGroup())
                ;
        }
        #endregion
    }
}
