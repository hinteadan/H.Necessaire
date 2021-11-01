using H.Necessaire;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class CoreDependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry.Register<HttpClient>(() => new HttpClient());
        }
    }
}
