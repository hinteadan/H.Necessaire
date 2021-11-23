﻿using H.Necessaire.Runtime.Resources.Concrete;
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
        }
    }
}
