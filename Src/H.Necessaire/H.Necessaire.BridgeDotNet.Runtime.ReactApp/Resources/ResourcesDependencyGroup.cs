namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class ResourcesDependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry.Register<SecurityResource>(() => new SecurityResource());
        }
    }
}
