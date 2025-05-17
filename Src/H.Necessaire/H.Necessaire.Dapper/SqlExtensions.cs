using System;

namespace H.Necessaire.Dapper
{
    public static class SqlExtensions
    {
        public static string WithDatabase(this string connectionString, string databaseName)
        {
            ConnectionStringBuilder connectionStringBuilder = new ConnectionStringBuilder(connectionString.WithoutDatabase());

            connectionStringBuilder.Set("Database", databaseName);

            return connectionStringBuilder.ToString();
        }

        public static string WithoutDatabase(this string connectionString)
        {
            ConnectionStringBuilder connectionStringBuilder = new ConnectionStringBuilder(connectionString);

            connectionStringBuilder.Zap("Database");
            connectionStringBuilder.Zap("InitialCatalog");

            return connectionStringBuilder.ToString();
        }

        public static string GetDatabaseName(this string connectionString)
        {
            string result = null;
            new Action(() => result = new ConnectionStringBuilder(connectionString).GetFirst("InitialCatalog", "Database")?.Value).TryOrFailWithGrace(onFail: ex => result = null);
            return result;
        }
    }
}
