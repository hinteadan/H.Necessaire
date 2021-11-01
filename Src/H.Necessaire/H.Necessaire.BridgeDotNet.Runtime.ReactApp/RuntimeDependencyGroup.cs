namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class RuntimeDependencyGroup : ImADependencyGroup
    {
        #region Construct
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry.Register<CoreDependencyGroup>(() => new CoreDependencyGroup());

            dependencyRegistry.Register<ResourcesDependencyGroup>(() => new ResourcesDependencyGroup());
            dependencyRegistry.Register<ManagersDependencyGroup>(() => new ManagersDependencyGroup());
        }
        #endregion
    }
}
