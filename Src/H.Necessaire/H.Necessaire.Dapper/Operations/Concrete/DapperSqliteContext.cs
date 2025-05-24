using Dapper;
using System.Data;
using System.Threading.Tasks;

namespace H.Necessaire.Dapper.Operations.Concrete
{
    public class DapperSqliteContext : DapperContextBase
    {
        public DapperSqliteContext(IDbConnection dbConnection, string defaultTableName = null)
            : base(dbConnection, defaultTableName) { }

        protected override string PrintLimitSyntax(int offset, int count)
            => $"LIMIT {count} OFFSET {offset}";

        public override async Task TruncateTable(string tableName = null)
        {
            string sql = $"DELETE FROM [{tableName ?? defaultTableName}]";
            await dbConnection.ExecuteAsync(sql);
        }
    }
}
