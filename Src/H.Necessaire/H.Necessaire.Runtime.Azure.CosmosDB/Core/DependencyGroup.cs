using H.Necessaire.Runtime.Azure.CosmosDB.Core.Resources;
using System;

namespace H.Necessaire.Runtime.Azure.CosmosDB.Core
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry

                .RegisterAlwaysNew<HsCosmosStorageService>(() => new HsCosmosStorageService())


                .Register<AuditMetadataAzureCosmosDbStorageResource>(() => new AuditMetadataAzureCosmosDbStorageResource())
                .Register<AuditPayloadAzureCosmosDbStorageResource>(() => new AuditPayloadAzureCosmosDbStorageResource())
                .Register<AuditAzureCosmosDbStorageResource>(() => new AuditAzureCosmosDbStorageResource())
                .Register<ImAnAuditingService>(() => dependencyRegistry.Get<AuditAzureCosmosDbStorageResource>())


                .Register<ConsumerIdentityAzureCosmosDbStorageResource>(() => new ConsumerIdentityAzureCosmosDbStorageResource())
                .Register<ImAStorageService<Guid, ConsumerIdentity>>(() => dependencyRegistry.Get<ConsumerIdentityAzureCosmosDbStorageResource>())
                .Register<ImAStorageBrowserService<ConsumerIdentity, IDFilter<Guid>>>(() => dependencyRegistry.Get<ConsumerIdentityAzureCosmosDbStorageResource>())

                .Register<LogEntryAzureCosmosDbStorageResource>(() => new LogEntryAzureCosmosDbStorageResource())
                .Register<ImAStorageService<Guid, LogEntry>>(() => dependencyRegistry.Get<LogEntryAzureCosmosDbStorageResource>())
                .Register<ImAStorageBrowserService<LogEntry, LogFilter>>(() => dependencyRegistry.Get<LogEntryAzureCosmosDbStorageResource>())

                .Register<ExiledSyncRequestAzureCosmosDbStorageResource>(() => new ExiledSyncRequestAzureCosmosDbStorageResource())
                .Register<ImAStorageService<string, ExiledSyncRequest>>(() => dependencyRegistry.Get<ExiledSyncRequestAzureCosmosDbStorageResource>())
                .Register<ImAStorageBrowserService<ExiledSyncRequest, SyncRequestFilter>>(() => dependencyRegistry.Get<ExiledSyncRequestAzureCosmosDbStorageResource>())

                .Register<NetworkTraceAzureCosmosDbStorageResource>(() => new NetworkTraceAzureCosmosDbStorageResource())
                .Register<ImAStorageService<Guid, NetworkTrace>>(() => dependencyRegistry.Get<NetworkTraceAzureCosmosDbStorageResource>())
                .Register<ImAStorageBrowserService<NetworkTrace, IDFilter<Guid>>>(() => dependencyRegistry.Get<NetworkTraceAzureCosmosDbStorageResource>())

                .Register<QdActionAzureCosmosDbStorageResource>(() => new QdActionAzureCosmosDbStorageResource())
                .Register<ImAStorageService<Guid, QdAction>>(() => dependencyRegistry.Get<QdActionAzureCosmosDbStorageResource>())
                .Register<ImAStorageBrowserService<QdAction, QdActionFilter>>(() => dependencyRegistry.Get<QdActionAzureCosmosDbStorageResource>())

                .Register<QdActionResultAzureCosmosDbStorageResource>(() => new QdActionResultAzureCosmosDbStorageResource())
                .Register<ImAStorageService<Guid, QdActionResult>>(() => dependencyRegistry.Get<QdActionResultAzureCosmosDbStorageResource>())
                .Register<ImAStorageBrowserService<QdActionResult, QdActionResultFilter>>(() => dependencyRegistry.Get<QdActionResultAzureCosmosDbStorageResource>())

                .Register<RuntimeTraceAzureCosmosDbStorageResource>(() => new RuntimeTraceAzureCosmosDbStorageResource())
                .Register<ImAStorageService<Guid, RuntimeTrace>>(() => dependencyRegistry.Get<RuntimeTraceAzureCosmosDbStorageResource>())
                .Register<ImAStorageBrowserService<RuntimeTrace, IDFilter<Guid>>>(() => dependencyRegistry.Get<RuntimeTraceAzureCosmosDbStorageResource>())

                .Register<SyncRequestAzureCosmosDbStorageResource>(() => new SyncRequestAzureCosmosDbStorageResource())
                .Register<ImAStorageService<string, SyncRequest>>(() => dependencyRegistry.Get<SyncRequestAzureCosmosDbStorageResource>())
                .Register<ImAStorageBrowserService<SyncRequest, SyncRequestFilter>>(() => dependencyRegistry.Get<SyncRequestAzureCosmosDbStorageResource>())

                .Register<AzureCosmosDbKeyValueStore>(() => new AzureCosmosDbKeyValueStore())
                .Register<IKeyValueStorage>(() => dependencyRegistry.Get<AzureCosmosDbKeyValueStore>())

                .Register<DataBinAzureCosmosDbStorageResource>(() => new DataBinAzureCosmosDbStorageResource())
                .Register<ImAStorageService<Guid, DataBinMeta>>(() => dependencyRegistry.Get<DataBinAzureCosmosDbStorageResource>())
                .Register<ImAStorageBrowserService<DataBinMeta, DataBinFilter>>(() => dependencyRegistry.Get<DataBinAzureCosmosDbStorageResource>())
                .Register<ImAStorageService<Guid, DataBin>>(() => dependencyRegistry.Get<DataBinAzureCosmosDbStorageResource>())
                .Register<ImAStorageBrowserService<DataBin, DataBinFilter>>(() => dependencyRegistry.Get<DataBinAzureCosmosDbStorageResource>())


                .Register<HsCosmosDebugger>(() => new HsCosmosDebugger())

                ;
        }
    }
}
