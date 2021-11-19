using Dapper;
using H.Necessaire.Dapper;
using H.Necessaire.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.SqlServer.Core.Resources
{
    internal class AuditSqlServerStorageResource : ImADependency, ImAnAuditingService
    {
        #region Construct
        AuditMetadataSqlServerStorageResource metadataStorage = null;
        AuditPayloadSqlServerStorageResource payloadStorage = null;

        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            metadataStorage = dependencyProvider.Get<AuditMetadataSqlServerStorageResource>();
            payloadStorage = dependencyProvider.Get<AuditPayloadSqlServerStorageResource>();
        }
        #endregion

        public async Task Append<T>(ImAnAuditEntry metadata, T objectSnapshot)
        {
            (await metadataStorage.Save(metadata.ToMeta())).ThrowOnFail();
            (await payloadStorage.Save(new AuditPayloadSqlServerStorageResource.AuditPayloadSqlEntry
            {
                ID = metadata.ID,
                PayloadAsJson = objectSnapshot.ToJsonObject(),
            })).ThrowOnFail();
        }

        public async Task<ImAnAuditEntry[]> Browse(AuditSearchFilter filter)
        {
            AuditMetadataEntry[] metadata = (await metadataStorage.LoadPage(filter)).ThrowOnFailOrReturn().Content;

            if (!metadata?.Any() ?? true)
                return new ImAnAuditEntry[0];

            return
                metadata
                .Select(x => new JsonAuditEntry(x, LoadAuditJsonPayload) as ImAnAuditEntry)
                .ToArray();
        }

        private async Task<Stream> LoadAuditJsonPayload(ImAnAuditEntry auditMetadata)
        {
            AuditPayloadSqlServerStorageResource.AuditPayloadSqlEntry auditPayloadEntry = (await payloadStorage.LoadByID(auditMetadata.ID)).ThrowOnFailOrReturn();
            return auditPayloadEntry?.PayloadAsJson?.ToStream();
        }
    }

    internal partial class AuditPayloadSqlServerStorageResource : DapperStorageServiceBase<Guid, AuditPayloadSqlServerStorageResource.AuditPayloadSqlEntry, AuditPayloadSqlServerStorageResource.AuditPayloadSqlEntry, IDFilter<Guid>>
    {
        #region Construct
        public AuditPayloadSqlServerStorageResource() : base(connectionString: null, tableName: auditPayloadTableName, databaseName: "H.Necessaire.Core") { }
        protected override Task<SqlMigration[]> GetAllMigrations() => sqlMigrations.AsTask();
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

    internal partial class AuditMetadataSqlServerStorageResource : DapperStorageServiceBase<Guid, AuditMetadataEntry, AuditMetadataSqlServerStorageResource.AuditMetadataSqlEntry, AuditSearchFilter>
    {
        #region Construct
        public AuditMetadataSqlServerStorageResource() : base(connectionString: null, tableName: auditMetaTableName, databaseName: "H.Necessaire.Core") { }
        protected override Task<SqlMigration[]> GetAllMigrations() => sqlMigrations.AsTask();
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
                        x.HappenedAtTicks = entity.HappenedAt.Ticks;
                        x.DoneByID = entity.DoneBy?.ID.ToString();
                        x.DoneByIDTag = entity.DoneBy?.IDTag;
                        x.DoneByDisplayName = entity.DoneBy?.DisplayName;
                        x.DoneByJson = entity.DoneBy?.ToJsonObject();
                        x.ActionTypeLabel = entity.ActionType.ToString();
                    });
            }
            public override AuditMetadataEntry MapSqlToEntity(AuditMetadataSqlEntry sqlEntity)
            {
                return
                    base.MapSqlToEntity(sqlEntity)
                    .And(x =>
                    {
                        x.HappenedAt = new DateTime(sqlEntity.HappenedAtTicks);
                        x.DoneBy = sqlEntity.DoneByJson?.JsonToObject<ConsumerIdentity>();
                    });
            }
        }
    }
}
