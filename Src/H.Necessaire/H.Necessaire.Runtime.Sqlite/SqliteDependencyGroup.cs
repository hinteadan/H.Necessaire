using H.Necessaire;
using H.Necessaire.Dapper;
using Microsoft.Data.Sqlite;

namespace H.Necessaire.Runtime.Sqlite
{
    public class SqliteDependencyGroup : DapperDependencyGroup
    {
        public SqliteDependencyGroup() : base(new SqlConnectionFactory(connectionString => new SqliteConnection(connectionString)), x => x.Build<ImASqlMigrationStore>("Sqlite")) { }

        public override void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            base.RegisterDependencies(dependencyRegistry);
        }
    }
}
