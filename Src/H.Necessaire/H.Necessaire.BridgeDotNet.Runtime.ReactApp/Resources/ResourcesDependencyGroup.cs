using System;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class ResourcesDependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry.Register<SecurityResource>(() => new SecurityResource());
            dependencyRegistry.Register<SyncRequestResource>(() => new SyncRequestResource());
            dependencyRegistry.Register<DeviceInfoResource>(() => new DeviceInfoResource());
            dependencyRegistry.Register<ConsumerIdentityLocalStorageResource>(() => new ConsumerIdentityLocalStorageResource());

            dependencyRegistry.Register<BffApiSyncRegistryResource>(() => new BffApiSyncRegistryResource());

            dependencyRegistry.Register<ImASyncRegistry[]>(() => new ImASyncRegistry[] {
                dependencyRegistry.Get<BffApiSyncRegistryResource>(),
            });

            dependencyRegistry.Register<ImASyncer<ConsumerIdentity, Guid>>(() => new ConsumerIdentitySyncerResource());
        }
    }
}
