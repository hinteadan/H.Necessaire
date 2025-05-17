using System.Threading.Tasks;

namespace H.Necessaire.Dapper
{
    public interface ImASqlMigrationStore
    {
        Task<OperationResult> Append(SqlMigration migration);

        Task<SqlMigration[]> Browse(SqlMigrationFilter filter);
    }
}
