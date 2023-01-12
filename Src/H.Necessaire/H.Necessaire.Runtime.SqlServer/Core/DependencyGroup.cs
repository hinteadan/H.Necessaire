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

            dependencyRegistry
                .Register<AuditMetadataSqlServerStorageResource>(() => new AuditMetadataSqlServerStorageResource())
                .Register<AuditPayloadSqlServerStorageResource>(() => new AuditPayloadSqlServerStorageResource())
                .Register<AuditSqlServerStorageResource>(() => new AuditSqlServerStorageResource())
                .Register<ImAnAuditingService>(() => dependencyRegistry.Get<AuditSqlServerStorageResource>())
                ;

            dependencyRegistry
                .Register<LogEntrySqlServerStorageResource>(() => new LogEntrySqlServerStorageResource())
                .Register<ImAStorageService<Guid, LogEntry>>(() => dependencyRegistry.Get<LogEntrySqlServerStorageResource>())
                .Register<ImAStorageBrowserService<LogEntry, LogFilter>>(() => dependencyRegistry.Get<LogEntrySqlServerStorageResource>())
                ;

            dependencyRegistry
                .Register<QdActionSqlServerStorageResource>(() => new QdActionSqlServerStorageResource())
                .Register<ImAStorageService<Guid, QdAction>>(() => dependencyRegistry.Get<QdActionSqlServerStorageResource>())
                .Register<ImAStorageBrowserService<QdAction, QdActionFilter>>(() => dependencyRegistry.Get<QdActionSqlServerStorageResource>())
                ;

            dependencyRegistry
                .Register<QdActionResultSqlServerStorageResource>(() => new QdActionResultSqlServerStorageResource())
                .Register<ImAStorageService<Guid, QdActionResult>>(() => dependencyRegistry.Get<QdActionResultSqlServerStorageResource>())
                .Register<ImAStorageBrowserService<QdActionResult, QdActionResultFilter>>(() => dependencyRegistry.Get<QdActionResultSqlServerStorageResource>())
                ;

            dependencyRegistry
                .Register<NetworkTraceSqlServerStorageResource>(() => new NetworkTraceSqlServerStorageResource())
                .Register<ImAStorageService<Guid, NetworkTrace>>(() => dependencyRegistry.Get<NetworkTraceSqlServerStorageResource>())
                .Register<ImAStorageBrowserService<NetworkTrace, IDFilter<Guid>>>(() => dependencyRegistry.Get<NetworkTraceSqlServerStorageResource>())
                ;

            dependencyRegistry
                .Register<RuntimeTraceSqlServerStorageResource>(() => new RuntimeTraceSqlServerStorageResource())
                .Register<ImAStorageService<Guid, RuntimeTrace>>(() => dependencyRegistry.Get<RuntimeTraceSqlServerStorageResource>())
                .Register<ImAStorageBrowserService<RuntimeTrace, IDFilter<Guid>>>(() => dependencyRegistry.Get<RuntimeTraceSqlServerStorageResource>())
                ;

            dependencyRegistry
               .Register<DataBinSqlServerStorageResource>(() => new DataBinSqlServerStorageResource())
               .Register<ImAStorageService<Guid, DataBinMeta>>(() => dependencyRegistry.Get<DataBinSqlServerStorageResource>())
               .Register<ImAStorageBrowserService<DataBinMeta, DataBinFilter>>(() => dependencyRegistry.Get<DataBinSqlServerStorageResource>())
               .Register<ImAStorageService<Guid, DataBin>>(() => dependencyRegistry.Get<DataBinSqlServerStorageResource>())
               .Register<ImAStorageBrowserService<DataBin, DataBinFilter>>(() => dependencyRegistry.Get<DataBinSqlServerStorageResource>())
               ;
        }
    }
}
