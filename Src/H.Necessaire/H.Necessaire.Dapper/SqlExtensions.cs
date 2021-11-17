using System;
using System.Data.SqlClient;

namespace H.Necessaire.Dapper
{
    public static class SqlExtensions
    {
        public static string WithDatabase(this string connectionString, string databaseName)
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString.WithoutDatabase());

            connectionStringBuilder.Add("Database", databaseName);

            return connectionStringBuilder.ToString();
        }

        public static string WithoutDatabase(this string connectionString)
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);

            connectionStringBuilder.Remove("Database");
            connectionStringBuilder.Remove("InitialCatalog");

            return connectionStringBuilder.ToString();
        }

        public static string GetDatabaseName(this string connectionString)
        {
            string result = null;
            new Action(() => result = new SqlConnectionStringBuilder(connectionString).InitialCatalog).TryOrFailWithGrace(onFail: ex => result = null);
            return result;
        }
    }
}
