﻿using Dapper;
using H.Necessaire.Dapper;
using H.Necessaire.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.SqlServer.Core.Resources
{
    internal partial class SyncRequestSqlServerStorageResource : DapperStorageServiceBase<string, SyncRequest, SyncRequestSqlServerStorageResource.SyncRequestSqlEntity, SyncRequestFilter>
    {
        #region Construct
        public SyncRequestSqlServerStorageResource() : base(connectionString: null, tableName: $"H.Necessaire.{nameof(SyncRequest)}", databaseName: "H.Necessaire.Sync") { }
        protected override Task<SqlMigration[]> GetAllMigrations() => sqlMigrations.AsTask();
        #endregion

        protected override ISqlFilterCriteria[] ApplyFilter(SyncRequestFilter filter, DynamicParameters sqlParams)
        {
            List<ISqlFilterCriteria> result = new List<ISqlFilterCriteria>();

            if (filter?.IDs?.Any() ?? false)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(SyncRequestSqlEntity.ID), parameterName: nameof(filter.IDs), @operator: "IN"));
            }

            if (filter?.PayloadIdentifiers?.Any() ?? false)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(SyncRequestSqlEntity.PayloadIdentifier), parameterName: nameof(filter.PayloadIdentifiers), @operator: "IN"));
            }

            if (filter?.PayloadTypes?.Any() ?? false)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(SyncRequestSqlEntity.PayloadType), parameterName: nameof(filter.PayloadTypes), @operator: "IN"));
            }

            if (filter?.SyncStatuses?.Any() ?? false)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(SyncRequestSqlEntity.SyncStatus), parameterName: nameof(filter.SyncStatuses), @operator: "IN"));
            }

            if (filter?.FromInclusive != null)
            {
                sqlParams.Add($"{nameof(filter.FromInclusive)}Ticks", filter.FromInclusive.Value.Ticks);
                result.Add(new SqlFilterCriteria(columnName: nameof(SyncRequestSqlEntity.HappenedAtTicks), parameterName: $"{nameof(filter.FromInclusive)}Ticks", @operator: ">="));
            }

            if (filter?.ToInclusive != null)
            {
                sqlParams.Add($"{nameof(filter.ToInclusive)}Ticks", filter.ToInclusive.Value.Ticks);
                result.Add(new SqlFilterCriteria(columnName: nameof(SyncRequestSqlEntity.HappenedAtTicks), parameterName: $"{nameof(filter.ToInclusive)}Ticks", @operator: "<="));
            }

            return result.ToArray();
        }

        public class SyncRequestSqlEntity : SqlEntryBase, IStringIdentity
        {
            public string ID { get; set; }

            public string Payload { get; set; }

            public string PayloadIdentifier { get; set; }

            public string PayloadType { get; set; }

            public SyncStatus SyncStatus { get; set; } = SyncStatus.NotSynced;

            public string SyncStatusLabel => SyncStatus.ToString();

            public DateTime HappenedAt { get; set; }

            public long HappenedAtTicks { get => HappenedAt.Ticks; set => HappenedAt = new DateTime(value); }

            public string OperationContextJson { get; set; }
        }

        public class SyncRequestSqlEntityMapper : SqlEntityMapperBase<SyncRequest, SyncRequestSqlEntity>
        {
            static SyncRequestSqlEntityMapper() => new SyncRequestSqlEntityMapper().RegisterMapper();

            public override SyncRequest MapSqlToEntity(SyncRequestSqlEntity sqlEntity)
            {
                return
                    base.MapSqlToEntity(sqlEntity)
                    .And(x =>
                    {
                        x.HappenedAt = new DateTime(sqlEntity.HappenedAtTicks);
                        x.OperationContext = sqlEntity.OperationContextJson?.JsonToObject<OperationContext>();
                    });
            }

            public override SyncRequestSqlEntity MapEntityToSql(SyncRequest entity)
            {
                return
                    base.MapEntityToSql(entity)
                    .And(x =>
                    {
                        x.OperationContextJson = entity.OperationContext?.ToJsonObject();
                    })
                    ;
            }
        }
    }
}
