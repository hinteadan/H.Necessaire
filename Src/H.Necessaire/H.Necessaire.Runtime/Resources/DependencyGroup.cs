using H.Necessaire.Runtime.Resources.Concrete;
using System;

namespace H.Necessaire.Runtime.Resources
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<SyncRequestFileSystemStorageResource>(() => new SyncRequestFileSystemStorageResource())
                .Register<ImAStorageService<string, SyncRequest>>(() => dependencyRegistry.Get<SyncRequestFileSystemStorageResource>())
                .Register<ImAStorageBrowserService<SyncRequest, SyncRequestFilter>>(() => dependencyRegistry.Get<SyncRequestFileSystemStorageResource>())
                ;

            dependencyRegistry
                .Register<ExiledSyncRequestFileSystemStorageResource>(() => new ExiledSyncRequestFileSystemStorageResource())
                .Register<ImAStorageService<string, ExiledSyncRequest>>(() => dependencyRegistry.Get<ExiledSyncRequestFileSystemStorageResource>())
                .Register<ImAStorageBrowserService<ExiledSyncRequest, SyncRequestFilter>>(() => dependencyRegistry.Get<ExiledSyncRequestFileSystemStorageResource>())
                ;

            dependencyRegistry
                .Register<ConsumerIdentityFileSystemStorageResource>(() => new ConsumerIdentityFileSystemStorageResource())
                .Register<ImAStorageService<Guid, ConsumerIdentity>>(() => dependencyRegistry.Get<ConsumerIdentityFileSystemStorageResource>())
                .Register<ImAStorageBrowserService<ConsumerIdentity, IDFilter<Guid>>>(() => dependencyRegistry.Get<ConsumerIdentityFileSystemStorageResource>())
                ;

            dependencyRegistry
                .Register<AuditMetadataFileSystemStorageResource>(() => new AuditMetadataFileSystemStorageResource())
                .Register<AuditFileSystemStorageResource>(() => new AuditFileSystemStorageResource())
                .Register<ImAnAuditingService>(() => dependencyRegistry.Get<AuditFileSystemStorageResource>())
                ;

            dependencyRegistry
                .Register<KeyValueFileSystemStorageResource>(() => new KeyValueFileSystemStorageResource())
                .Register<IKeyValueStorage>(() => dependencyRegistry.Get<KeyValueFileSystemStorageResource>())
                ;

            dependencyRegistry
                .Register<LogEntryFileSystemStorageResource>(() => new LogEntryFileSystemStorageResource())
                .Register<ImAStorageService<Guid, LogEntry>>(() => dependencyRegistry.Get<LogEntryFileSystemStorageResource>())
                .Register<ImAStorageBrowserService<LogEntry, LogFilter>>(() => dependencyRegistry.Get<LogEntryFileSystemStorageResource>())
                ;

            dependencyRegistry
                .Register<QdActionFileSystemStorageResource>(() => new QdActionFileSystemStorageResource())
                .Register<ImAStorageService<Guid, QdAction>>(() => dependencyRegistry.Get<QdActionFileSystemStorageResource>())
                .Register<ImAStorageBrowserService<QdAction, QdActionFilter>>(() => dependencyRegistry.Get<QdActionFileSystemStorageResource>())
                ;

            dependencyRegistry
                .Register<QdActionResultFileSystemStorageResource>(() => new QdActionResultFileSystemStorageResource())
                .Register<ImAStorageService<Guid, QdActionResult>>(() => dependencyRegistry.Get<QdActionResultFileSystemStorageResource>())
                .Register<ImAStorageBrowserService<QdActionResult, QdActionResultFilter>>(() => dependencyRegistry.Get<QdActionResultFileSystemStorageResource>())
                ;

            dependencyRegistry
                .Register<NetworkTraceFileSystemStorageResource>(() => new NetworkTraceFileSystemStorageResource())
                .Register<ImAStorageService<Guid, NetworkTrace>>(() => dependencyRegistry.Get<NetworkTraceFileSystemStorageResource>())
                .Register<ImAStorageBrowserService<NetworkTrace, IDFilter<Guid>>>(() => dependencyRegistry.Get<NetworkTraceFileSystemStorageResource>())
                ;

            dependencyRegistry
                .Register<RuntimeTraceFileSystemStorageResource>(() => new RuntimeTraceFileSystemStorageResource())
                .Register<ImAStorageService<Guid, RuntimeTrace>>(() => dependencyRegistry.Get<RuntimeTraceFileSystemStorageResource>())
                .Register<ImAStorageBrowserService<RuntimeTrace, IDFilter<Guid>>>(() => dependencyRegistry.Get<RuntimeTraceFileSystemStorageResource>())
                ;

            dependencyRegistry
                .Register<DataBinMetaFileSystemStorageResource>(() => new DataBinMetaFileSystemStorageResource())
                .Register<ImAStorageService<Guid, DataBinMeta>>(() => dependencyRegistry.Get<DataBinMetaFileSystemStorageResource>())
                .Register<ImAStorageBrowserService<DataBinMeta, DataBinFilter>>(() => dependencyRegistry.Get<DataBinMetaFileSystemStorageResource>())
                ;

            dependencyRegistry
                .Register<DataBinFileSystemStorageResource>(() => new DataBinFileSystemStorageResource())
                .Register<ImAStorageService<Guid, DataBin>>(() => dependencyRegistry.Get<DataBinFileSystemStorageResource>())
                .Register<ImAStorageBrowserService<DataBin, DataBinFilter>>(() => dependencyRegistry.Get<DataBinFileSystemStorageResource>())
                ;
        }
    }
}
