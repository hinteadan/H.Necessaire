using H.Necessaire.Runtime.RavenDB.Core.Resources;
using System;

namespace H.Necessaire.Runtime.RavenDB.Core
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<Versioning.DepsGroup>(() => new Versioning.DepsGroup())
                ;

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
                .Register<AuditMetadataRavenDbStorageResource>(() => new AuditMetadataRavenDbStorageResource())
                .Register<AuditPayloadRavenDbStorageResource>(() => new AuditPayloadRavenDbStorageResource())
                .Register<AuditRavenDbStorageResource>(() => new AuditRavenDbStorageResource())
                .Register<ImAnAuditingService>(() => dependencyRegistry.Get<AuditRavenDbStorageResource>())
                ;

            dependencyRegistry
                .Register<RavenDbKeyValueStore>(() => new RavenDbKeyValueStore())
                .Register<IKeyValueStorage>(() => dependencyRegistry.Get<RavenDbKeyValueStore>())
                ;

            dependencyRegistry
                .Register<LogEntryRavenDbStorageResource>(() => new LogEntryRavenDbStorageResource())
                .Register<ImAStorageService<Guid, LogEntry>>(() => dependencyRegistry.Get<LogEntryRavenDbStorageResource>())
                .Register<ImAStorageBrowserService<LogEntry, LogFilter>>(() => dependencyRegistry.Get<LogEntryRavenDbStorageResource>())
                ;

            dependencyRegistry
                .Register<QdActionRavenDbStorageResource>(() => new QdActionRavenDbStorageResource())
                .Register<ImAStorageService<Guid, QdAction>>(() => dependencyRegistry.Get<QdActionRavenDbStorageResource>())
                .Register<ImAStorageBrowserService<QdAction, QdActionFilter>>(() => dependencyRegistry.Get<QdActionRavenDbStorageResource>())
                ;

            dependencyRegistry
                .Register<QdActionResultRavenDbStorageResource>(() => new QdActionResultRavenDbStorageResource())
                .Register<ImAStorageService<Guid, QdActionResult>>(() => dependencyRegistry.Get<QdActionResultRavenDbStorageResource>())
                .Register<ImAStorageBrowserService<QdActionResult, QdActionResultFilter>>(() => dependencyRegistry.Get<QdActionResultRavenDbStorageResource>())
                ;

            dependencyRegistry
                .Register<NetworkTraceRavenDbStorageResource>(() => new NetworkTraceRavenDbStorageResource())
                .Register<ImAStorageService<Guid, NetworkTrace>>(() => dependencyRegistry.Get<NetworkTraceRavenDbStorageResource>())
                .Register<ImAStorageBrowserService<NetworkTrace, IDFilter<Guid>>>(() => dependencyRegistry.Get<NetworkTraceRavenDbStorageResource>())
                ;

            dependencyRegistry
                .Register<RuntimeTraceRavenDbStorageResource>(() => new RuntimeTraceRavenDbStorageResource())
                .Register<ImAStorageService<Guid, RuntimeTrace>>(() => dependencyRegistry.Get<RuntimeTraceRavenDbStorageResource>())
                .Register<ImAStorageBrowserService<RuntimeTrace, IDFilter<Guid>>>(() => dependencyRegistry.Get<RuntimeTraceRavenDbStorageResource>())
                ;

            dependencyRegistry
                .Register<DataBinRavenDbStorageResource>(() => new DataBinRavenDbStorageResource())
                .Register<ImAStorageService<Guid, DataBinMeta>>(() => dependencyRegistry.Get<DataBinRavenDbStorageResource>())
                .Register<ImAStorageBrowserService<DataBinMeta, DataBinFilter>>(() => dependencyRegistry.Get<DataBinRavenDbStorageResource>())
                .Register<ImAStorageService<Guid, DataBin>>(() => dependencyRegistry.Get<DataBinRavenDbStorageResource>())
                .Register<ImAStorageBrowserService<DataBin, DataBinFilter>>(() => dependencyRegistry.Get<DataBinRavenDbStorageResource>())
                ;
        }
    }
}
