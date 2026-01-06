namespace H.Necessaire.Runtime.RavenDB.Core.Versioning
{
    internal class DepsGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<RavenDbReleaseVersionStore>(() => new RavenDbReleaseVersionStore())
                .Register<ImAReleaseVersionStore>(() => dependencyRegistry.Get<RavenDbReleaseVersionStore>())
                .Register<ImAReleaseVersionProvider>(() => dependencyRegistry.Get<RavenDbReleaseVersionStore>())
                ;
        }
    }
}
