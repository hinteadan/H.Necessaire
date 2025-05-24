using System;

namespace H.Necessaire.Runtime.Sqlite.Core.Resources
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<SyncRequestSqliteRsx>(() => new SyncRequestSqliteRsx())
                .Register<ImAStorageService<string, SyncRequest>>(() => dependencyRegistry.Get<SyncRequestSqliteRsx>())
                .Register<ImAStorageBrowserService<SyncRequest, SyncRequestFilter>>(() => dependencyRegistry.Get<SyncRequestSqliteRsx>())
                ;

            dependencyRegistry
                .Register<ExiledSyncRequestSqliteRsx>(() => new ExiledSyncRequestSqliteRsx())
                .Register<ImAStorageService<string, ExiledSyncRequest>>(() => dependencyRegistry.Get<ExiledSyncRequestSqliteRsx>())
                .Register<ImAStorageBrowserService<ExiledSyncRequest, SyncRequestFilter>>(() => dependencyRegistry.Get<ExiledSyncRequestSqliteRsx>())
                ;

            dependencyRegistry
                .Register<ConsumerIdentitySqliteRsx>(() => new ConsumerIdentitySqliteRsx())
                .Register<ImAStorageService<Guid, ConsumerIdentity>>(() => dependencyRegistry.Get<ConsumerIdentitySqliteRsx>())
                .Register<ImAStorageBrowserService<ConsumerIdentity, IDFilter<Guid>>>(() => dependencyRegistry.Get<ConsumerIdentitySqliteRsx>())
                ;

            dependencyRegistry
                .Register<LogEntrySqliteRsx>(() => new LogEntrySqliteRsx())
                .Register<ImAStorageService<Guid, LogEntry>>(() => dependencyRegistry.Get<LogEntrySqliteRsx>())
                .Register<ImAStorageBrowserService<LogEntry, LogFilter>>(() => dependencyRegistry.Get<LogEntrySqliteRsx>())
                ;

            dependencyRegistry
                .Register<LogEntrySqliteRsx>(() => new LogEntrySqliteRsx())
                .Register<ImAStorageService<Guid, LogEntry>>(() => dependencyRegistry.Get<LogEntrySqliteRsx>())
                .Register<ImAStorageBrowserService<LogEntry, LogFilter>>(() => dependencyRegistry.Get<LogEntrySqliteRsx>())
                ;

            dependencyRegistry
               .Register<QdActionSqliteRsx>(() => new QdActionSqliteRsx())
               .Register<ImAStorageService<Guid, QdAction>>(() => dependencyRegistry.Get<QdActionSqliteRsx>())
               .Register<ImAStorageBrowserService<QdAction, QdActionFilter>>(() => dependencyRegistry.Get<QdActionSqliteRsx>())
               ;

            dependencyRegistry
                .Register<QdActionResultSqliteRsx>(() => new QdActionResultSqliteRsx())
                .Register<ImAStorageService<Guid, QdActionResult>>(() => dependencyRegistry.Get<QdActionResultSqliteRsx>())
                .Register<ImAStorageBrowserService<QdActionResult, QdActionResultFilter>>(() => dependencyRegistry.Get<QdActionResultSqliteRsx>())
                ;

            dependencyRegistry
               .Register<DataBinSqliteRsx>(() => new DataBinSqliteRsx())
               .Register<ImAStorageService<Guid, DataBinMeta>>(() => dependencyRegistry.Get<DataBinSqliteRsx>())
               .Register<ImAStorageBrowserService<DataBinMeta, DataBinFilter>>(() => dependencyRegistry.Get<DataBinSqliteRsx>())
               .Register<ImAStorageService<Guid, DataBin>>(() => dependencyRegistry.Get<DataBinSqliteRsx>())
               .Register<ImAStorageBrowserService<DataBin, DataBinFilter>>(() => dependencyRegistry.Get<DataBinSqliteRsx>())
               ;

            dependencyRegistry.Register<Auditing.DependencyGroup>(() => new Auditing.DependencyGroup());
        }
    }
}
