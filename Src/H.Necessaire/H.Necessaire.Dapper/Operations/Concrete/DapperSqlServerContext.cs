using System.Data;

namespace H.Necessaire.Dapper.Operations.Concrete
{
    public class DapperSqlServerContext : DapperContextBase
    {
        public DapperSqlServerContext(IDbConnection dbConnection, string defaultTableName = null) : base(dbConnection, defaultTableName) { }
    }
}
