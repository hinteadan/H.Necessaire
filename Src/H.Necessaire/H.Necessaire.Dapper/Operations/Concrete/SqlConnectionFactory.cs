using System;
using System.Data;

namespace H.Necessaire.Dapper
{
    public class SqlConnectionFactory : ImASqlConnectionFactory
    {
        readonly Func<string, IDbConnection> dbConnectionFactory;
        public SqlConnectionFactory(Func<string, IDbConnection> dbConnectionFactory)
        {
            this.dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory), $"The connection factory method must be provided");
        }

        public IDbConnection BuildNewConnection(string connectionString)
            => dbConnectionFactory(connectionString);
    }
}
