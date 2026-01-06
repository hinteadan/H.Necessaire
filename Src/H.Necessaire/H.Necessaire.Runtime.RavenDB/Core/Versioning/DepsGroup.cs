namespace H.Necessaire.Runtime.RavenDB.Core.Versioning
{
    internal class DepsGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<ImAReleaseVersionStore>(() => new RavenDbReleaseVersionStore())
                ;
        }
    }
}
