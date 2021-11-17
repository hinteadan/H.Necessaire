using H.Necessaire.Runtime.RavenDB.Core.Resources;
using System;

namespace H.Necessaire.Runtime.RavenDB.Core
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<SyncRequestRavenDbStorageResource>(() => new SyncRequestRavenDbStorageResource())
                .Register<ImAStorageService<string, SyncRequest>>(() => dependencyRegistry.Get<SyncRequestRavenDbStorageResource>())
                .Register<ImAStorageBrowserService<SyncRequest, SyncRequestFilter>>(() => dependencyRegistry.Get<SyncRequestRavenDbStorageResource>())
                ;

            dependencyRegistry
                .Register<ExiledSyncRequestRavenDbStorageResource>(() => new ExiledSyncRequestRavenDbStorageResource())
                .Register<ImAStorageService<string, ExiledSyncRequest>>(() => dependencyRegistry.Get<ExiledSyncRequestRavenDbStorageResource>())
                .Register<ImAStorageBrowserService<ExiledSyncRequest, SyncRequestFilter>>(() => dependencyRegistry.Get<ExiledSyncRequestRavenDbStorageResource>())
                ;

            dependencyRegistry
                .Register<ConsumerIdentityRavenDbStorageResource>(() => new ConsumerIdentityRavenDbStorageResource())
                .Register<ImAStorageService<Guid, ConsumerIdentity>>(() => dependencyRegistry.Get<ConsumerIdentityRavenDbStorageResource>())
                .Register<ImAStorageBrowserService<ConsumerIdentity, IDFilter<Guid>>>(() => dependencyRegistry.Get<ConsumerIdentityRavenDbStorageResource>())
                ;

            dependencyRegistry
                .Register<RavenDbKeyValueStore>(() => new RavenDbKeyValueStore())
                .Register<IKeyValueStorage>(() => dependencyRegistry.Get<RavenDbKeyValueStore>())
                ;
        }
    }
}
