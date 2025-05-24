using Dapper;
using H.Necessaire.Dapper.Operations.Concrete;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace H.Necessaire.Dapper
{
    public abstract class DapperSqlServerResourceBase : DapperResourceBase
    {
        #region Construct
        ImASqlConnectionFactory sqlConnectionFactory = null;
        bool isDatabaseEnsured = false;
        protected DapperSqlServerResourceBase(string connectionString = null, string tableName = null, string databaseName = null)
            : base(connectionString, tableName, databaseName) { }

        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            sqlConnectionFactory = dependencyProvider.Get<ImASqlConnectionFactory>();
            if (sqlConnectionFactory is null)
                throw new ArgumentNullException(nameof(sqlConnectionFactory), $"A concrete implementation for {nameof(ImASqlConnectionFactory)} must be registered within the current dependecy registry");
            base.ReferDependencies(dependencyProvider);
        }
        #endregion

        protected override async Task EnsureDatabase()
        {
            if (isDatabaseEnsured)
                return;

            bool databaseExists = false;

            await
                new Func<Task>(async () =>
                {

                    using (IDbConnection dbConnection = sqlConnectionFactory.BuildNewConnection(connectionString))
                    {
                        databaseExists = await dbConnection.ExecuteScalarAsync<bool>($"SELECT CASE WHEN ISNULL(DB_ID('{databaseName}'), 0) = 0 THEN 0 ELSE 1 END");
                        if (databaseExists)
                        {
                            isDatabaseEnsured = true;
                            return;
                        }

                        await dbConnection.ExecuteAsync($"CREATE DATABASE [{databaseName}]");
                    }

                }).TryOrFailWithGrace(onFail: ex => databaseExists = false);

            if (!databaseExists)
            {
                await
                    new Func<Task>(async () =>
                    {

                        using (IDbConnection dbConnection = sqlConnectionFactory.BuildNewConnection(connectionStringWithoutDatabase))
                        {
                            databaseExists = await dbConnection.ExecuteScalarAsync<bool>($"SELECT CASE WHEN ISNULL(DB_ID('{databaseName}'), 0) = 0 THEN 0 ELSE 1 END");
                            if (databaseExists)
                            {
                                isDatabaseEnsured = true;
                                return;
                            }

                            await dbConnection.ExecuteAsync($"CREATE DATABASE [{databaseName}]");
                        }

                    }).TryOrFailWithGrace(onFail: ex => databaseExists = false);
            }

            isDatabaseEnsured = true;
        }

        protected override ImADapperContext NewDbContext(string tableName = null)
        {
            return new DapperSqlServerContext(sqlConnectionFactory.BuildNewConnection(connectionString), tableName ?? this.tableName);
        }
    }
}
