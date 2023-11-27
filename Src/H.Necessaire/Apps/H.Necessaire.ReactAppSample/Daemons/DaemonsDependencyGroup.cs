using H.Necessaire.BridgeDotNet.Runtime.ReactApp;

namespace H.Necessaire.ReactAppSample
{
    internal class DaemonsDependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {

            dependencyRegistry

                //.Register<ConsolePingDaemon>(() => new ConsolePingDaemon())
                //.Register<ConsolePingDaemon.Worker>(() => new ConsolePingDaemon.Worker())

                .Register<ImADaemon[]>(() =>
                    new ImADaemon[] {
                        //dependencyRegistry.Get<ConsolePingDaemon>(),
                        dependencyRegistry.Get<SyncDaemon>(),
                        dependencyRegistry.Get<SecurityContextUpdateDaemon>(),
                    }
                );
        }
    }
}
