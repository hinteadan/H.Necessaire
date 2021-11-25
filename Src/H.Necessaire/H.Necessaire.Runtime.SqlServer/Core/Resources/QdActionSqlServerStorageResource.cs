using Dapper;
using H.Necessaire.Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.SqlServer.Core.Resources
{
    internal partial class QdActionSqlServerStorageResource : DapperStorageServiceBase<Guid, QdAction, QdActionSqlServerStorageResource.QdActionSqlEntry, QdActionFilter>
    {
        #region Construct
        public QdActionSqlServerStorageResource() : base(connectionString: null, tableName: qdActionTableName, databaseName: "H.Necessaire.Core") { }
        protected override Task<SqlMigration[]> GetAllMigrations() => sqlMigrations.AsTask();
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
                        x.QdAtTicks = entity.QdAt.Ticks;
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
                        x.QdAt = new DateTime(sqlEntity.QdAtTicks);
                    })
                    ;
            }
        }
    }
}
