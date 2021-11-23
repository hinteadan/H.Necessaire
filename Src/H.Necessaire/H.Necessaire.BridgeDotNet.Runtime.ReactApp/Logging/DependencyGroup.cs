namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp.Logging
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<PersistentLogProcessor>(() => new PersistentLogProcessor())
                ;
        }
    }
}
