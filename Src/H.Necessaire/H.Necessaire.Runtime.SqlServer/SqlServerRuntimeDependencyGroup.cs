using H.Necessaire.Dapper;
using Microsoft.Data.SqlClient;

namespace H.Necessaire.Runtime.SqlServer
{
    public class SqlServerRuntimeDependencyGroup : DapperDependencyGroup
    {
        public SqlServerRuntimeDependencyGroup() : base(new SqlConnectionFactory(connectionString => new SqlConnection(connectionString))) { }

        public override void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            base.RegisterDependencies(dependencyRegistry);

            dependencyRegistry
                .Register<Core.DependencyGroup>(() => new Core.DependencyGroup())
                .Register<Security.DependencyGroup>(() => new Security.DependencyGroup())
                .Register<Analytics.DependencyGroup>(() => new Analytics.DependencyGroup())
                ;

        }
    }
}
