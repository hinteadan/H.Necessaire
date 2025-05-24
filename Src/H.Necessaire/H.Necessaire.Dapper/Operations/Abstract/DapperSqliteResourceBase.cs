using H.Necessaire.Dapper.Operations.Concrete;
using System;
using System.Data;
using System.IO;
using System.Reflection;
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

            HSafe.Run(() => {
                ConnectionStringBuilder connectionStringBuilder = new ConnectionStringBuilder(connectionString);
                string rawPath = connectionStringBuilder.Get("Data Source")?.Value;
                FileInfo databaseFile = new FileInfo(rawPath);
                DirectoryInfo databaseFolder = databaseFile.Directory;
                databaseFolder.Create();
            });

            using (IDbConnection db = sqlConnectionFactory.BuildNewConnection(connectionString)) { }

            isDatabaseEnsured = true;

            return Task.CompletedTask;
        }

        protected override ImADapperContext NewDbContext(string tableName = null)
        {
            return new DapperSqliteContext(sqlConnectionFactory.BuildNewConnection(connectionString), tableName ?? this.tableName);
        }

        protected void ProcessSqliteConnectionString()
        {
            if (connectionString.IsEmpty())
                return;

            if (databaseName.IsEmpty())
                return;

            ConnectionStringBuilder connectionStringBuilder = new ConnectionStringBuilder(connectionString).Zap("Database");
            connectionString = connectionStringBuilder.ToString();

            string rawPath = connectionStringBuilder.Get("Data Source")?.Value;

            if (rawPath.IsEmpty())
            {
                connectionStringBuilder.Set("Data Source", $"{databaseName}.sqlite3");
                connectionString = connectionStringBuilder.ToString();
                return;
            }

            HSafe.Run(() =>
            {

                FileInfo databaseFile = new FileInfo(rawPath);
                DirectoryInfo databaseFolder = databaseFile.Directory;

                string dbFileName = databaseFile.Name;
                string dbExtension = Path.GetExtension(dbFileName);
                string dbName = Path.GetFileNameWithoutExtension(dbFileName);

                if (databaseName.Is(dbName))
                    return;

                string newDbPath = Path.Combine(databaseFolder.FullName, $"{databaseName}{dbExtension}");

                connectionStringBuilder.Set("Data Source", newDbPath);

                connectionString = connectionStringBuilder.ToString();

            }, "process Sqlite ConnectionString");
        }
    }
}
