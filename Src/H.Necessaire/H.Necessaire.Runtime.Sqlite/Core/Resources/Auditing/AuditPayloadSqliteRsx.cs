using Dapper;
using H.Necessaire.Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Sqlite.Core.Resources.Auditing
{
    internal class AuditPayloadSqliteRsx
        : DapperSqliteStorageResourceBase<
            Guid,
            AuditPayloadSqliteRsx.AuditPayloadSqlEntry,
            AuditPayloadSqliteRsx.AuditPayloadSqlEntry,
            IDFilter<Guid>
        >
    {
        #region Construct
        static SqlMigration[] sqlMigrations = null;
        public AuditPayloadSqliteRsx() : base(connectionString: null, tableName: "H.Necessaire.AuditPayload", databaseName: "H.Necessaire.Core") { }
        protected override async Task<SqlMigration[]> GetAllMigrations()
        {
            if (sqlMigrations != null)
                return sqlMigrations;

            sqlMigrations = new SqlMigration[] {
                await new SqlMigration{
                    ResourceIdentifier = "AuditPayload",
                    VersionNumber = new VersionNumber(1, 0),
                }.AndAsync(async x => x.SqlCommand = await ReadMigrationSqlCommand(x, typeof(SqliteDependencyGroup).Assembly)),
            };

            return sqlMigrations;
        }
        #endregion

        protected override ISqlFilterCriteria[] ApplyFilter(IDFilter<Guid> filter, DynamicParameters sqlParams)
        {
            List<ISqlFilterCriteria> result = new List<ISqlFilterCriteria>();

            if (filter?.IDs?.Any() ?? false)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(AuditPayloadSqlEntry.ID), parameterName: nameof(filter.IDs), @operator: "IN"));
            }

            return result.ToArray();
        }
        public class AuditPayloadSqlEntry : SqlEntryBase, IGuidIdentity
        {
            public Guid ID { get; set; }
            public string PayloadAsJson { get; set; }
        }

        public class AuditPayloadSqlEntryMapper : SqlEntityMapperBase<AuditPayloadSqlEntry, AuditPayloadSqlEntry>
        {
            static AuditPayloadSqlEntryMapper() => new AuditPayloadSqlEntryMapper().RegisterMapper();
        }
    }
}
