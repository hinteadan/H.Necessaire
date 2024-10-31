using H.Necessaire.Runtime.Google.FirestoreDB.Core.Resources;
using System;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Core
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry

                .RegisterAlwaysNew<HsFirestoreStorageService>(() => new HsFirestoreStorageService())


                .Register<AuditMetadataGoogleFirestoreDbStorageResource>(() => new AuditMetadataGoogleFirestoreDbStorageResource())
                .Register<AuditPayloadGoogleFirestoreDbStorageResource>(() => new AuditPayloadGoogleFirestoreDbStorageResource())
                .Register<AuditGoogleFirestoreDbStorageResource>(() => new AuditGoogleFirestoreDbStorageResource())
                .Register<ImAnAuditingService>(() => dependencyRegistry.Get<AuditGoogleFirestoreDbStorageResource>())


                .Register<ConsumerIdentityGoogleFirestoreDbStorageResource>(() => new ConsumerIdentityGoogleFirestoreDbStorageResource())
                .Register<ImAStorageService<Guid, ConsumerIdentity>>(() => dependencyRegistry.Get<ConsumerIdentityGoogleFirestoreDbStorageResource>())
                .Register<ImAStorageBrowserService<ConsumerIdentity, IDFilter<Guid>>>(() => dependencyRegistry.Get<ConsumerIdentityGoogleFirestoreDbStorageResource>())

                .Register<LogEntryGoogleFirestoreDbStorageResource>(() => new LogEntryGoogleFirestoreDbStorageResource())
                .Register<ImAStorageService<Guid, LogEntry>>(() => dependencyRegistry.Get<LogEntryGoogleFirestoreDbStorageResource>())
                .Register<ImAStorageBrowserService<LogEntry, LogFilter>>(() => dependencyRegistry.Get<LogEntryGoogleFirestoreDbStorageResource>())

                .Register<ExiledSyncRequestGoogleFirestoreDbStorageResource>(() => new ExiledSyncRequestGoogleFirestoreDbStorageResource())
                .Register<ImAStorageService<string, ExiledSyncRequest>>(() => dependencyRegistry.Get<ExiledSyncRequestGoogleFirestoreDbStorageResource>())
                .Register<ImAStorageBrowserService<ExiledSyncRequest, SyncRequestFilter>>(() => dependencyRegistry.Get<ExiledSyncRequestGoogleFirestoreDbStorageResource>())

                .Register<NetworkTraceGoogleFirestoreDbStorageResource>(() => new NetworkTraceGoogleFirestoreDbStorageResource())
                .Register<ImAStorageService<Guid, NetworkTrace>>(() => dependencyRegistry.Get<NetworkTraceGoogleFirestoreDbStorageResource>())
                .Register<ImAStorageBrowserService<NetworkTrace, IDFilter<Guid>>>(() => dependencyRegistry.Get<NetworkTraceGoogleFirestoreDbStorageResource>())

                .Register<QdActionGoogleFirestoreDbStorageResource>(() => new QdActionGoogleFirestoreDbStorageResource())
                .Register<ImAStorageService<Guid, QdAction>>(() => dependencyRegistry.Get<QdActionGoogleFirestoreDbStorageResource>())
                .Register<ImAStorageBrowserService<QdAction, QdActionFilter>>(() => dependencyRegistry.Get<QdActionGoogleFirestoreDbStorageResource>())

                .Register<QdActionResultGoogleFirestoreDbStorageResource>(() => new QdActionResultGoogleFirestoreDbStorageResource())
                .Register<ImAStorageService<Guid, QdActionResult>>(() => dependencyRegistry.Get<QdActionResultGoogleFirestoreDbStorageResource>())
                .Register<ImAStorageBrowserService<QdActionResult, QdActionResultFilter>>(() => dependencyRegistry.Get<QdActionResultGoogleFirestoreDbStorageResource>())

                .Register<RuntimeTraceGoogleFirestoreDbStorageResource>(() => new RuntimeTraceGoogleFirestoreDbStorageResource())
                .Register<ImAStorageService<Guid, RuntimeTrace>>(() => dependencyRegistry.Get<RuntimeTraceGoogleFirestoreDbStorageResource>())
                .Register<ImAStorageBrowserService<RuntimeTrace, IDFilter<Guid>>>(() => dependencyRegistry.Get<RuntimeTraceGoogleFirestoreDbStorageResource>())

                .Register<SyncRequestGoogleFirestoreDbStorageResource>(() => new SyncRequestGoogleFirestoreDbStorageResource())
                .Register<ImAStorageService<string, SyncRequest>>(() => dependencyRegistry.Get<SyncRequestGoogleFirestoreDbStorageResource>())
                .Register<ImAStorageBrowserService<SyncRequest, SyncRequestFilter>>(() => dependencyRegistry.Get<SyncRequestGoogleFirestoreDbStorageResource>())

                .Register<GoogleFirestoreDbKeyValueStore>(() => new GoogleFirestoreDbKeyValueStore())
                .Register<IKeyValueStorage>(() => dependencyRegistry.Get<GoogleFirestoreDbKeyValueStore>())

                .Register<DataBinGoogleFirestoreDbStorageResource>(() => new DataBinGoogleFirestoreDbStorageResource())
                .Register<ImAStorageService<Guid, DataBinMeta>>(() => dependencyRegistry.Get<DataBinGoogleFirestoreDbStorageResource>())
                .Register<ImAStorageBrowserService<DataBinMeta, DataBinFilter>>(() => dependencyRegistry.Get<DataBinGoogleFirestoreDbStorageResource>())
                .Register<ImAStorageService<Guid, DataBin>>(() => dependencyRegistry.Get<DataBinGoogleFirestoreDbStorageResource>())
                .Register<ImAStorageBrowserService<DataBin, DataBinFilter>>(() => dependencyRegistry.Get<DataBinGoogleFirestoreDbStorageResource>())


                //.Register<HsFirestoreDebugger>(() => new HsFirestoreDebugger())

                ;
        }
    }
}
