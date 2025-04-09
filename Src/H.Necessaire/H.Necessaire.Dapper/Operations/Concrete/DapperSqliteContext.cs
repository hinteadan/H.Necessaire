using System.Data;

namespace H.Necessaire.Dapper.Operations.Concrete
{
    public class DapperSqliteContext : DapperContextBase
    {
        public DapperSqliteContext(IDbConnection dbConnection, string defaultTableName = null)
            : base(dbConnection, defaultTableName) { }

        
    }
}
