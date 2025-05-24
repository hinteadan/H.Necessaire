using H.Necessaire.Dapper.Operations.Concrete;
using System;

namespace H.Necessaire.Dapper
{
    public class DapperDependencyGroup : ImADependencyGroup
    {
        readonly ImASqlConnectionFactory sqlConnectionFactory = null;
        readonly Func<ImADependencyProvider, ImASqlMigrationStore> migrationStoreProvider = null;
        public DapperDependencyGroup(ImASqlConnectionFactory sqlConnectionFactory = null, Func<ImADependencyProvider, ImASqlMigrationStore> migrationStoreProvider = null)
        {
            this.sqlConnectionFactory = sqlConnectionFactory;
            this.migrationStoreProvider = migrationStoreProvider;
        }

        public virtual void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            if (sqlConnectionFactory != null)
                dependencyRegistry.Register<ImASqlConnectionFactory>(() => sqlConnectionFactory);

            dependencyRegistry
                .Register<SqlConnectionRegistry>(() => new SqlConnectionRegistry())
                .Register<ImASqlEntityConnectionProvider>(() => dependencyRegistry.Get<SqlConnectionRegistry>())
                .Register<ImASqlEntityConnectionRegistry>(() => dependencyRegistry.Get<SqlConnectionRegistry>())
                ;

            dependencyRegistry
                .Register<IKeyValueStorage>(() => new SqlKeyValueStore())
                .Register<SqliteMigrationStore>(() => new SqliteMigrationStore())
                .Register<SqlServerMigrationStore>(() => new SqlServerMigrationStore())
                ;

            if (migrationStoreProvider != null)
                dependencyRegistry.Register<ImASqlMigrationStore>(() => migrationStoreProvider.Invoke(dependencyRegistry));
            else
                dependencyRegistry.Register<ImASqlMigrationStore>(() => dependencyRegistry.Get<SqlServerMigrationStore>());

            MappingExtensions.InitializeHNecessaireDapperMappers();
        }
    }
}
