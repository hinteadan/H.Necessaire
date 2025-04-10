using H.Necessaire.Dapper.Operations.Concrete;
using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace H.Necessaire.Dapper
{
    public abstract class DapperSqliteResourceBase : DapperResourceBase
    {
        #region Construct
        bool isDatabaseEnsured = false;
        ImASqlConnectionFactory sqlConnectionFactory = null;
        protected DapperSqliteResourceBase(string connectionString = null, string tableName = null, string databaseName = null)
            : base(connectionString, tableName, databaseName)
        {
            this.connectionString = connectionString;
            this.tableName = tableName;
            this.databaseName = databaseName;

            ProcessSqliteConnectionString();
        }

        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            sqlConnectionFactory = dependencyProvider.Get<ImASqlConnectionFactory>();
            if (sqlConnectionFactory is null)
                throw new ArgumentNullException(nameof(sqlConnectionFactory), $"A concrete implementation for {nameof(ImASqlConnectionFactory)} must be registered within the current dependecy registry");

            base.ReferDependencies(dependencyProvider);

            ProcessSqliteConnectionString();
        }
        #endregion

        protected override Task EnsureDatabase()
        {
            if (isDatabaseEnsured)
                return Task.CompletedTask;

            using (IDbConnection db = sqlConnectionFactory.BuildNewConnection(connectionString)) { }

            isDatabaseEnsured = true;

            return Task.CompletedTask;
        }

        protected override ImADapperContext NewDbContext(string tableName = null)
        {
            return new DapperSqliteContext(sqlConnectionFactory.BuildNewConnection(connectionString), tableName ?? this.tableName);
        }

        void ProcessSqliteConnectionString()
        {
            string rawPath = new ConnectionStringBuilder(connectionString).GetFirst("Data Source")?.Value;

            if (rawPath.IsEmpty())
                return;

            HSafe.Run(() => {

                FileInfo databaseFile = new FileInfo(Uri.UnescapeDataString(new UriBuilder(rawPath).Path));
                DirectoryInfo databaseFolder = databaseFile.Directory;



            }, "process Sqlite ConnectionString");
        }
    }
}
