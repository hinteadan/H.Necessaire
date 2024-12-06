using H.Necessaire.BridgeDotNet.Runtime.ReactApp.Resources;
using H.Necessaire.BridgeDotNet.Runtime.ReactApp.Resources.Logging;
using H.Necessaire.BridgeDotNet.Runtime.ReactApp.Resources.Sync;
using H.Necessaire.BridgeDotNet.Runtime.ReactApp.Resources.Versioning;
using System;
using System.Linq;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class ResourcesDependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<VersionResource>(() => new VersionResource())
                .Register<ImAVersionProvider>(() => dependencyRegistry.Get<VersionResource>())
                .Register<SecurityResource>(() => new SecurityResource())
                .Register<SyncRequestResource>(() => new SyncRequestResource())
                .Register<DeviceInfoResource>(() => new DeviceInfoResource())
                .Register<ConsumerIdentityLocalStorageResource>(() => new ConsumerIdentityLocalStorageResource())
                ;

            dependencyRegistry
                .Register<LogEntryLocalStorageResource>(() => new LogEntryLocalStorageResource())
                .Register<ImAStorageService<Guid, LogEntry>>(() => dependencyRegistry.Get<LogEntryLocalStorageResource>())
                .Register<ImAStorageBrowserService<LogEntry, LogFilter>>(() => dependencyRegistry.Get<LogEntryLocalStorageResource>())
                ;

            dependencyRegistry
                .Register<BffApiSyncRegistryResource>(() => new BffApiSyncRegistryResource())
                .Register<ImASyncRegistry[]>(() => new ImASyncRegistry[] {
                    dependencyRegistry.Get<BffApiSyncRegistryResource>(),
                });

            dependencyRegistry
                .Register<ImASyncer<ConsumerIdentity, Guid>>(() => new ConsumerIdentitySyncerResource())
                .Register<ImASyncer<LogEntry, Guid>>(() => new LogEntrySyncerResource())
                ;

            dependencyRegistry
                .Register<Resources.AppState.DependencyGroup>(() => new Resources.AppState.DependencyGroup())
                ;

            dependencyRegistry
                .RegisterAlwaysNew<ImAUserPrivateDataStorage[]>(() =>
                {
                    return
                        typeof(ImAUserPrivateDataStorage)
                        .GetAllImplementations()
                        .Select(t => dependencyRegistry.Get(t) as ImAUserPrivateDataStorage)
                        .ToNoNullsArray()
                        ;
                })
                ;
        }
    }
}
