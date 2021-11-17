using H.Necessaire.Runtime.SqlServer.Core.Resources;
using System;

namespace H.Necessaire.Runtime.SqlServer.Core
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<SyncRequestSqlServerStorageResource>(() => new SyncRequestSqlServerStorageResource())
                .Register<ImAStorageService<string, SyncRequest>>(() => dependencyRegistry.Get<SyncRequestSqlServerStorageResource>())
                .Register<ImAStorageBrowserService<SyncRequest, SyncRequestFilter>>(() => dependencyRegistry.Get<SyncRequestSqlServerStorageResource>())
                ;

            dependencyRegistry
                .Register<ExiledSyncRequestSqlServerStorageResource>(() => new ExiledSyncRequestSqlServerStorageResource())
                .Register<ImAStorageService<string, ExiledSyncRequest>>(() => dependencyRegistry.Get<ExiledSyncRequestSqlServerStorageResource>())
                .Register<ImAStorageBrowserService<ExiledSyncRequest, SyncRequestFilter>>(() => dependencyRegistry.Get<ExiledSyncRequestSqlServerStorageResource>())
                ;

            dependencyRegistry
                .Register<ConsumerIdentitySqlServerStorageResource>(() => new ConsumerIdentitySqlServerStorageResource())
                .Register<ImAStorageService<Guid, ConsumerIdentity>>(() => dependencyRegistry.Get<ConsumerIdentitySqlServerStorageResource>())
                .Register<ImAStorageBrowserService<ConsumerIdentity, IDFilter<Guid>>>(() => dependencyRegistry.Get<ConsumerIdentitySqlServerStorageResource>())
                ;
        }
    }
}
