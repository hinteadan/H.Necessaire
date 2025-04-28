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

            dependencyRegistry.Register<Auditing.DependencyGroup>(() => new Auditing.DependencyGroup());
        }
    }
}
