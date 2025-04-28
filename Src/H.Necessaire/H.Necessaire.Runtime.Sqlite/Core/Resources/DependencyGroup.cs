using System;

namespace H.Necessaire.Runtime.Sqlite.Core.Resources
{
    internal class DependencyGroup : ImADependencyGroup
    {
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
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
