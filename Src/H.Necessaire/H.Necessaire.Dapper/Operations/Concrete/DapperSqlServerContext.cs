using System.Data;

namespace H.Necessaire.Dapper.Operations.Concrete
{
    public class DapperSqlServerContext : DapperContextBase
    {
        public DapperSqlServerContext(IDbConnection dbConnection, string defaultTableName = null)
            : base(dbConnection, defaultTableName) { }

        protected override string PrintLimitSyntax(int offset, int count)
            => $"OFFSET {offset} ROWS FETCH NEXT {count} ROWS ONLY";
    }
}
