using Dapper;
using H.Necessaire.Dapper;
using Microsoft.Data.Sqlite;
using System;

namespace H.Necessaire.Runtime.Sqlite
{
    public class SqliteDependencyGroup : DapperDependencyGroup
    {
        public SqliteDependencyGroup() : base(new SqlConnectionFactory(connectionString => new SqliteConnection(connectionString)), x => x.Build<ImASqlMigrationStore>("Sqlite")) { }

        public override void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            SqlMapper.AddTypeHandler(new SqliteGuidTypeHandler());
            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.RemoveTypeMap(typeof(Guid?));

            SqlMapper.AddTypeHandler(new SqliteTimespanTypeHandler());
            SqlMapper.RemoveTypeMap(typeof(TimeSpan));
            SqlMapper.RemoveTypeMap(typeof(TimeSpan?));

            base.RegisterDependencies(dependencyRegistry);

            dependencyRegistry
                .Register<Core.DependencyGroup>(() => new Core.DependencyGroup())
                //.Register<Security.DependencyGroup>(() => new Security.DependencyGroup())
                //.Register<Analytics.DependencyGroup>(() => new Analytics.DependencyGroup())
                ;
        }
    }
}
