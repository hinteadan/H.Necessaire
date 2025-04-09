using System.Data;

namespace H.Necessaire.Dapper.Operations.Concrete
{
    public class DapperSqliteContext : DapperContextBase
    {
        public DapperSqliteContext(IDbConnection dbConnection, string defaultTableName = null)
            : base(dbConnection, defaultTableName) { }

        protected override string PrintLimitSyntax(int offset, int count)
            => $"LIMIT {count} OFFSET {offset}";
    }
}
