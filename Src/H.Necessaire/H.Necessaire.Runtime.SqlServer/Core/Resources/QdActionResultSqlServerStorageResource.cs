using Dapper;
using H.Necessaire.Dapper;
using H.Necessaire.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.SqlServer.Core.Resources
{
    internal partial class QdActionResultSqlServerStorageResource : DapperStorageServiceBase<Guid, QdActionResult, QdActionResultSqlServerStorageResource.QdActionResultSqlEntry, QdActionResultFilter>
    {
        #region Construct
        public QdActionResultSqlServerStorageResource() : base(connectionString: null, tableName: qdActionResultTableName, databaseName: "H.Necessaire.Core") { }
        protected override Task<SqlMigration[]> GetAllMigrations() => sqlMigrations.AsTask();
        #endregion

        protected override ISqlFilterCriteria[] ApplyFilter(QdActionResultFilter filter, DynamicParameters sqlParams)
        {
            List<ISqlFilterCriteria> result = new List<ISqlFilterCriteria>();

            if (filter?.IDs?.Any() ?? false)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(QdActionResultSqlEntry.ID), parameterName: nameof(filter.IDs), @operator: "IN"));
            }

            if (filter?.QdActionIDs?.Any() ?? false)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(QdActionResultSqlEntry.QdActionID), parameterName: nameof(filter.QdActionIDs), @operator: "IN"));
            }

            if (filter?.FromInclusive != null)
            {
                sqlParams.Add($"{nameof(filter.FromInclusive)}Ticks", filter.FromInclusive.Value.Ticks);
                result.Add(new SqlFilterCriteria(columnName: nameof(QdActionResultSqlEntry.HappenedAtTicks), parameterName: $"{nameof(filter.FromInclusive)}Ticks", @operator: ">="));
            }

            if (filter?.ToInclusive != null)
            {
                sqlParams.Add($"{nameof(filter.ToInclusive)}Ticks", filter.ToInclusive.Value.Ticks);
                result.Add(new SqlFilterCriteria(columnName: nameof(QdActionResultSqlEntry.HappenedAtTicks), parameterName: $"{nameof(filter.ToInclusive)}Ticks", @operator: "<="));
            }

            return result.ToArray();
        }

        public class QdActionResultSqlEntry : SqlEntryBase, IGuidIdentity
        {
            public Guid ID { get; set; }
            public Guid QdActionID { get; set; }
            public DateTime HappenedAt { get; set; }
            public long HappenedAtTicks { get; set; }
            public string QdActionJson { get; set; }
            public bool IsSuccessful { get; set; }
            public string Reason { get; set; }
            public string CommentsJson { get; set; }
        }

        public class QdActionResultSqlEntryMapper : SqlEntityMapperBase<QdActionResult, QdActionResultSqlEntry>
        {
            static QdActionResultSqlEntryMapper() => new QdActionResultSqlEntryMapper().RegisterMapper();

            public override QdActionResultSqlEntry MapEntityToSql(QdActionResult entity)
            {
                return
                    base
                    .MapEntityToSql(entity)
                    .And(x =>
                    {
                        x.HappenedAtTicks = entity.HappenedAt.Ticks;
                        x.QdActionJson = entity.Payload.ToJsonObject();
                        x.CommentsJson = entity.Comments.ToJsonArray();
                    })
                    ;
            }

            public override QdActionResult MapSqlToEntity(QdActionResultSqlEntry sqlEntity)
            {
                return
                    base
                    .MapSqlToEntity(sqlEntity)
                    .And(x =>
                    {
                        x.HappenedAt = new DateTime(sqlEntity.HappenedAtTicks);
                        x.Payload = sqlEntity.QdActionJson.JsonToObject<QdAction>();
                        x.Comments = sqlEntity.CommentsJson.JsonToObject<string[]>();
                    })
                    ;
            }
        }
    }
}
