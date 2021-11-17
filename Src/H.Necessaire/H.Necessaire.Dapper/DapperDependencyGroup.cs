using H.Necessaire.Dapper.Operations.Concrete;

namespace H.Necessaire.Dapper
{
    public class DapperDependencyGroup : ImADependencyGroup
    {
        public virtual void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry
                .Register<SqlConnectionRegistry>(() => new SqlConnectionRegistry())
                .Register<ImASqlEntityConnectionProvider>(() => dependencyRegistry.Get<SqlConnectionRegistry>())
                .Register<ImASqlEntityConnectionRegistry>(() => dependencyRegistry.Get<SqlConnectionRegistry>())
                ;

            dependencyRegistry
                .Register<IKeyValueStorage>(() => new SqlKeyValueStore())
                .Register<ImASqlMigrationStore>(() => new SqlMigrationStore())
                ;

            MappingExtensions.InitializeHNecessaireDapperMappers();
        }
    }
}
