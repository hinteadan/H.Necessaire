using Dapper;
using H.Necessaire.Dapper;
using H.Necessaire.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Sqlite.Core.Resources
{
    internal class QdActionSqliteRsx
        : DapperSqliteStorageResourceBase<
            Guid,
            QdAction,
            QdActionSqliteRsx.QdActionSqlEntry,
            QdActionFilter
        >
    {
        #region Construct
        static SqlMigration[] sqlMigrations = null;
        public QdActionSqliteRsx() : base(connectionString: null, tableName: "H.Necessaire.QdAction", databaseName: "H.Necessaire.Core") { }
        protected override async Task<SqlMigration[]> GetAllMigrations()
        {
            if (sqlMigrations != null)
                return sqlMigrations;

            sqlMigrations = new SqlMigration[] {
                await new SqlMigration{
                    ResourceIdentifier = "QdAction",
                    VersionNumber = new VersionNumber(1, 0),
                }.AndAsync(async x => x.SqlCommand = await ReadMigrationSqlCommand(x, typeof(SqliteDependencyGroup).Assembly)),
            };

            return sqlMigrations;
        }
        #endregion

        protected override ISqlFilterCriteria[] ApplyFilter(QdActionFilter filter, DynamicParameters sqlParams)
        {
            List<ISqlFilterCriteria> result = new List<ISqlFilterCriteria>();

            if (filter?.IDs?.Any() ?? false)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(QdActionSqlEntry.ID), parameterName: nameof(filter.IDs), @operator: "IN"));
            }

            if (filter?.Types?.Any() ?? false)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(QdActionSqlEntry.Type), parameterName: nameof(filter.Types), @operator: "IN"));
            }

            if (filter?.Statuses?.Any() ?? false)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(QdActionSqlEntry.Status), parameterName: nameof(filter.Statuses), @operator: "IN"));
            }

            if (filter?.MinRunCount != null)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(QdActionSqlEntry.RunCount), parameterName: $"{nameof(filter.MinRunCount)}", @operator: ">="));
            }

            if (filter?.MaxRunCount != null)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(QdActionSqlEntry.RunCount), parameterName: $"{nameof(filter.MaxRunCount)}", @operator: "<="));
            }

            if (filter?.FromInclusive != null)
            {
                sqlParams.Add($"{nameof(filter.FromInclusive)}Ticks", filter.FromInclusive.Value.Ticks);
                result.Add(new SqlFilterCriteria(columnName: nameof(QdActionSqlEntry.QdAtTicks), parameterName: $"{nameof(filter.FromInclusive)}Ticks", @operator: ">="));
            }

            if (filter?.ToInclusive != null)
            {
                sqlParams.Add($"{nameof(filter.ToInclusive)}Ticks", filter.ToInclusive.Value.Ticks);
                result.Add(new SqlFilterCriteria(columnName: nameof(QdActionSqlEntry.QdAtTicks), parameterName: $"{nameof(filter.ToInclusive)}Ticks", @operator: "<="));
            }

            return result.ToArray();
        }

        public class QdActionSqlEntry : SqlEntryBase, IGuidIdentity
        {
            public Guid ID { get; set; }
            public DateTime QdAt { get; set; }
            public long QdAtTicks { get; set; }
            public string Type { get; set; }
            public string Payload { get; set; }
            public QdActionStatus Status { get; set; }
            public string StatusLabel { get; set; }
            public int RunCount { get; set; }
        }

        public class QdActionSqlEntryMapper : SqlEntityMapperBase<QdAction, QdActionSqlEntry>
        {
            static QdActionSqlEntryMapper() => new QdActionSqlEntryMapper().RegisterMapper();

            public override QdActionSqlEntry MapEntityToSql(QdAction entity)
            {
                return
                    base
                    .MapEntityToSql(entity)
                    .And(x =>
                    {
                        x.QdAtTicks = entity.QdAt.EnsureUtc().Ticks;
                        x.StatusLabel = entity.Status.ToString();
                    })
                    ;
            }

            public override QdAction MapSqlToEntity(QdActionSqlEntry sqlEntity)
            {
                return
                    base
                    .MapSqlToEntity(sqlEntity)
                    .And(x =>
                    {
                        x.QdAt = new DateTime(sqlEntity.QdAtTicks, DateTimeKind.Utc);
                    })
                    ;
            }
        }
    }
}
