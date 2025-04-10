using H.Necessaire.Dapper.Operations.Concrete;

namespace H.Necessaire.Dapper
{
    public class DapperDependencyGroup : ImADependencyGroup
    {
        readonly ImASqlConnectionFactory sqlConnectionFactory = null;
        public DapperDependencyGroup(ImASqlConnectionFactory sqlConnectionFactory = null)
        {
            this.sqlConnectionFactory = sqlConnectionFactory;
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
                .Register<ImASqlMigrationStore>(() => new SqlServerMigrationStore())
                ;

            MappingExtensions.InitializeHNecessaireDapperMappers();
        }
    }
}
