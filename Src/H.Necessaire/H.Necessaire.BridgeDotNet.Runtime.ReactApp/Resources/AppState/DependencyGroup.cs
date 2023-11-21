using H.Necessaire.BridgeDotNet.Runtime.ReactApp.Core.Model.AppState;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp.Resources.AppState
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<SecurityContextAppStateResource>(() => new SecurityContextAppStateResource())
                .Register<ImAStorageService<string, SecurityContextAppStateEntry>>(() => dependencyRegistry.Get<SecurityContextAppStateResource>())
                ;
        }
    }
}
