using H.Necessaire.Dapper.Operations.Concrete;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.Dapper
{
    public abstract class DapperSqliteResourceBase : DapperResourceBase
    {
        #region Construct
        ImASqlConnectionFactory sqlConnectionFactory = null;
        protected DapperSqliteResourceBase(string connectionString = null, string tableName = null, string databaseName = null)
            : base(connectionString, tableName, databaseName) { }

        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            sqlConnectionFactory = dependencyProvider.Get<ImASqlConnectionFactory>();
            if (sqlConnectionFactory is null)
                throw new ArgumentNullException(nameof(sqlConnectionFactory), $"A concrete implementation for {nameof(ImASqlConnectionFactory)} must be registered within the current dependecy registry");
            base.ReferDependencies(dependencyProvider);
        }
        #endregion

        protected override Task EnsureDatabase() => Task.CompletedTask;

        protected override ImADapperContext NewDbContext(string tableName = null)
        {
            return new DapperSqliteContext(sqlConnectionFactory.BuildNewConnection(connectionString), tableName ?? this.tableName);
        }
    }
}
