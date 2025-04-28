using H.Necessaire.Runtime.Sqlite.Core.Resources;
using System;

namespace H.Necessaire.Runtime.Sqlite.Core
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
        }
    }
}
