namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class ManagersDependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry.Register<SecurityManager>(() => new SecurityManager());
            dependencyRegistry.Register<ConsumerIdentityManager>(() => new ConsumerIdentityManager());
        }
    }
}
