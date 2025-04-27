using System.Data;

namespace H.Necessaire.Dapper
{
    public interface ImASqlConnectionFactory
    {
        IDbConnection BuildNewConnection(string connectionString);
    }
}
