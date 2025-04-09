using System.Data;

namespace H.Necessaire.Dapper.Operations.Concrete
{
    public class DapperSqlContext : DapperContextBase
    {
        public DapperSqlContext(IDbConnection dbConnection, string defaultTableName = null) : base(dbConnection, defaultTableName) { }
    }
}
