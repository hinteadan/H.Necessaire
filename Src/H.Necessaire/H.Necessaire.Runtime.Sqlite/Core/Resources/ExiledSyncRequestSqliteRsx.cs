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
    internal class ExiledSyncRequestSqliteRsx
        : DapperSqliteStorageResourceBase<
            string,
            ExiledSyncRequest,
            ExiledSyncRequestSqliteRsx.ExiledSyncRequestSqlEntity,
            SyncRequestFilter
        >
    {
        #region Construct
        static SqlMigration[] sqlMigrations = null;
        public ExiledSyncRequestSqliteRsx() : base(connectionString: null, tableName: "H.Necessaire.ExiledSyncRequest", databaseName: "H.Necessaire.Core") { }

        protected override async Task<SqlMigration[]> GetAllMigrations()
        {
            if (sqlMigrations != null)
                return sqlMigrations;

            sqlMigrations = new SqlMigration[] {
                await new SqlMigration{
                    ResourceIdentifier = nameof(ExiledSyncRequest),
                    VersionNumber = new VersionNumber(1, 0),
                }.AndAsync(async x => x.SqlCommand = await ReadMigrationSqlCommand(x, typeof(SqliteDependencyGroup).Assembly)),
            };

            return sqlMigrations;
        }
        #endregion
        protected override ISqlFilterCriteria[] ApplyFilter(SyncRequestFilter filter, DynamicParameters sqlParams)
        {
            List<ISqlFilterCriteria> result = new List<ISqlFilterCriteria>();

            if (filter?.IDs?.Any() ?? false)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(ExiledSyncRequestSqlEntity.ID), parameterName: nameof(filter.IDs), @operator: "IN"));
            }

            if (filter?.PayloadIdentifiers?.Any() ?? false)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(ExiledSyncRequestSqlEntity.PayloadIdentifier), parameterName: nameof(filter.PayloadIdentifiers), @operator: "IN"));
            }

            if (filter?.PayloadTypes?.Any() ?? false)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(ExiledSyncRequestSqlEntity.PayloadType), parameterName: nameof(filter.PayloadTypes), @operator: "IN"));
            }

            if (filter?.SyncStatuses?.Any() ?? false)
            {
                result.Add(new ComposedSqlFilterCriteria(
                    filter
                    .SyncStatuses
                    .Select(
                        (s, index) =>
                        {
                            sqlParams.Add($"{nameof(SyncRequest.SyncStatus)}{index}", $":{(int)s}");
                            return
                                new SqlFilterCriteria(
                                    columnName: nameof(ExiledSyncRequestSqlEntity.SyncRequestJson),
                                    parameterName: $"{nameof(SyncRequest.SyncStatus)}{index}",
                                    @operator: "like"
                                );
                        }
                    ).ToArray()
                ));
            }

            if (filter?.FromInclusive != null)
            {
                sqlParams.Add($"{nameof(filter.FromInclusive)}Ticks", filter.FromInclusive.Value.Ticks);
                result.Add(new SqlFilterCriteria(columnName: nameof(ExiledSyncRequestSqlEntity.HappenedAtTicks), parameterName: $"{nameof(filter.FromInclusive)}Ticks", @operator: ">="));
            }

            if (filter?.ToInclusive != null)
            {
                sqlParams.Add($"{nameof(filter.ToInclusive)}Ticks", filter.ToInclusive.Value.Ticks);
                result.Add(new SqlFilterCriteria(columnName: nameof(ExiledSyncRequestSqlEntity.HappenedAtTicks), parameterName: $"{nameof(filter.ToInclusive)}Ticks", @operator: "<="));
            }

            return result.ToArray();
        }

        public class ExiledSyncRequestSqlEntity : SqlEntryBase, IStringIdentity
        {
            public string ID { get; set; }

            public string PayloadIdentifier { get; set; }

            public string PayloadType { get; set; }

            public DateTime HappenedAt { get; set; }

            public long HappenedAtTicks { get => HappenedAt.Ticks; set => HappenedAt = new DateTime(value); }

            public string SyncRequestJson { get; set; }

            public string SyncRequestProcessingResultJson { get; set; }
        }

        public class ExiledSyncRequestSqlEntityMapper : SqlEntityMapperBase<ExiledSyncRequest, ExiledSyncRequestSqlEntity>
        {
            static ExiledSyncRequestSqlEntityMapper() => new ExiledSyncRequestSqlEntityMapper().RegisterMapper();

            public override ExiledSyncRequest MapSqlToEntity(ExiledSyncRequestSqlEntity sqlEntity)
            {
                return
                    base.MapSqlToEntity(sqlEntity)
                    .And(x =>
                    {
                        x.HappenedAt = new DateTime(sqlEntity.HappenedAtTicks, DateTimeKind.Utc);
                        x.SyncRequest = sqlEntity.SyncRequestJson?.JsonToObject<SyncRequest>();
                        x.SyncRequestProcessingResult = sqlEntity.SyncRequestProcessingResultJson?.JsonToObject<OperationResult>();
                    });
            }

            public override ExiledSyncRequestSqlEntity MapEntityToSql(ExiledSyncRequest entity)
            {
                return
                    base.MapEntityToSql(entity)
                    .And(x =>
                    {
                        x.ID = entity.ID;
                        x.HappenedAtTicks = x.HappenedAt.EnsureUtc().Ticks;
                        x.SyncRequestJson = entity.SyncRequest?.ToJsonObject();
                        x.SyncRequestProcessingResultJson = entity.SyncRequestProcessingResult?.ToJsonObject();
                    })
                    ;
            }
        }
    }
}
