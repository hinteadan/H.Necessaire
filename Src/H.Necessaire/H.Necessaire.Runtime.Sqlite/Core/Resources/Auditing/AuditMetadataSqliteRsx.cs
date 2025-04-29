using Dapper;
using H.Necessaire.Dapper;
using H.Necessaire.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Sqlite.Core.Resources.Auditing
{
    internal class AuditMetadataSqliteRsx
        : DapperSqliteStorageResourceBase<
            Guid,
            AuditMetadataEntry,
            AuditMetadataSqliteRsx.AuditMetadataSqlEntry,
            AuditSearchFilter
        >
    {
        #region Construct
        static SqlMigration[] sqlMigrations = null;
        public AuditMetadataSqliteRsx() : base(connectionString: null, tableName: "H.Necessaire.Audit", databaseName: "H.Necessaire.Core") { }
        protected override async Task<SqlMigration[]> GetAllMigrations()
        {
            if (sqlMigrations != null)
                return sqlMigrations;

            sqlMigrations = new SqlMigration[] {
                await new SqlMigration{
                    ResourceIdentifier = "Audit",
                    VersionNumber = new VersionNumber(1, 0),
                }.AndAsync(async x => x.SqlCommand = await ReadMigrationSqlCommand(x, typeof(SqliteDependencyGroup).Assembly)),
            };

            return sqlMigrations;
        }
        #endregion

        protected override ISqlFilterCriteria[] ApplyFilter(AuditSearchFilter filter, DynamicParameters sqlParams)
        {
            List<ISqlFilterCriteria> result = new List<ISqlFilterCriteria>();

            if (filter?.IDs?.Any() ?? false)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(AuditMetadataSqlEntry.ID), parameterName: nameof(filter.IDs), @operator: "IN"));
            }

            if (filter?.AuditedObjectTypes?.Any() ?? false)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(AuditMetadataSqlEntry.AuditedObjectType), parameterName: nameof(filter.AuditedObjectTypes), @operator: "IN"));
            }

            if (filter?.AuditedObjectIDs?.Any() ?? false)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(AuditMetadataSqlEntry.AuditedObjectID), parameterName: nameof(filter.AuditedObjectIDs), @operator: "IN"));
            }

            if (filter?.ActionTypes?.Any() ?? false)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(AuditMetadataSqlEntry.ActionType), parameterName: nameof(filter.ActionTypes), @operator: "IN"));
            }

            if (filter?.DoneBy?.IDs?.Any() ?? false)
            {
                sqlParams.Add("DoneByIDs", filter.DoneBy.IDs);
                result.Add(new SqlFilterCriteria(columnName: nameof(AuditMetadataSqlEntry.DoneByID), parameterName: "DoneByIDs", @operator: "IN"));
            }

            if (filter?.DoneBy?.IDTags?.Any() ?? false)
            {
                sqlParams.Add("DoneByIDTags", filter.DoneBy.IDTags);
                result.Add(new SqlFilterCriteria(columnName: nameof(AuditMetadataSqlEntry.DoneByIDTag), parameterName: "DoneByIDTags", @operator: "IN"));
            }

            if (filter?.DoneBy?.DisplayNames?.Any() ?? false)
            {
                sqlParams.Add("DoneByDisplayNames", filter.DoneBy.DisplayNames);
                result.Add(new SqlFilterCriteria(columnName: nameof(AuditMetadataSqlEntry.DoneByDisplayName), parameterName: "DoneByDisplayNames", @operator: "IN"));
            }

            if (filter?.FromInclusive != null)
            {
                sqlParams.Add($"{nameof(filter.FromInclusive)}Ticks", filter.FromInclusive.Value.Ticks);
                result.Add(new SqlFilterCriteria(columnName: nameof(AuditMetadataSqlEntry.HappenedAtTicks), parameterName: $"{nameof(filter.FromInclusive)}Ticks", @operator: ">="));
            }

            if (filter?.ToInclusive != null)
            {
                sqlParams.Add($"{nameof(filter.ToInclusive)}Ticks", filter.ToInclusive.Value.Ticks);
                result.Add(new SqlFilterCriteria(columnName: nameof(AuditMetadataSqlEntry.HappenedAtTicks), parameterName: $"{nameof(filter.ToInclusive)}Ticks", @operator: "<="));
            }

            return result.ToArray();
        }
        public class AuditMetadataSqlEntry : SqlEntryBase, IGuidIdentity
        {
            public Guid ID { get; set; }
            public string AuditedObjectType { get; set; }
            public string AuditedObjectID { get; set; }
            public DateTime HappenedAt { get; set; }
            public long HappenedAtTicks { get; set; }
            public string DoneByID { get; set; }
            public string DoneByIDTag { get; set; }
            public string DoneByDisplayName { get; set; }
            public string DoneByJson { get; set; }
            public AuditActionType ActionType { get; set; }
            public string ActionTypeLabel { get; set; }
            public string AppVersionJson { get; set; }
            public string AppVersionNumber { get; set; }
            public DateTime? AppVersionTimestamp { get; set; }
            public string AppVersionBranch { get; set; }
            public string AppVersionCommit { get; set; }
        }

        public class AuditMetadataSqlEntryMapper : SqlEntityMapperBase<AuditMetadataEntry, AuditMetadataSqlEntry>
        {
            static AuditMetadataSqlEntryMapper() => new AuditMetadataSqlEntryMapper().RegisterMapper();

            public override AuditMetadataSqlEntry MapEntityToSql(AuditMetadataEntry entity)
            {
                return
                    base.MapEntityToSql(entity)
                    .And(x =>
                    {
                        x.HappenedAtTicks = entity.HappenedAt.EnsureUtc().Ticks;
                        x.DoneByID = entity.DoneBy?.ID.ToString();
                        x.DoneByIDTag = entity.DoneBy?.IDTag;
                        x.DoneByDisplayName = entity.DoneBy?.DisplayName;
                        x.DoneByJson = entity.DoneBy?.ToJsonObject();
                        x.ActionTypeLabel = entity.ActionType.ToString();
                        x.AppVersionJson = entity.AppVersion?.ToJsonObject();
                        x.AppVersionNumber = entity.AppVersion?.Number?.ToString();
                        x.AppVersionTimestamp = entity.AppVersion?.Timestamp;
                        x.AppVersionBranch = entity.AppVersion?.Branch;
                        x.AppVersionCommit = entity.AppVersion?.Commit;
                    })
                    .And(x =>
                    {
                        if (x.AppVersionTimestamp == DateTime.MinValue) x.AppVersionTimestamp = null;
                    });
            }
            public override AuditMetadataEntry MapSqlToEntity(AuditMetadataSqlEntry sqlEntity)
            {
                return
                    base.MapSqlToEntity(sqlEntity)
                    .And(x =>
                    {
                        x.HappenedAt = new DateTime(sqlEntity.HappenedAtTicks, DateTimeKind.Utc);
                        x.DoneBy = sqlEntity.DoneByJson?.JsonToObject<ConsumerIdentity>();
                        x.AppVersion = sqlEntity.AppVersionJson?.JsonToObject<Version>();
                    });
            }
        }
    }
}
