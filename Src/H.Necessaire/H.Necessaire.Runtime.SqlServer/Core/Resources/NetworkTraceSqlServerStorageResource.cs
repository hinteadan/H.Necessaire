using Dapper;
using H.Necessaire.Dapper;
using H.Necessaire.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.SqlServer.Core.Resources
{
    internal partial class NetworkTraceSqlServerStorageResource : DapperStorageServiceBase<Guid, NetworkTrace, NetworkTraceSqlServerStorageResource.NetworkTraceSqlEntry, IDFilter<Guid>>
    {
        #region Construct
        public NetworkTraceSqlServerStorageResource() : base(connectionString: null, tableName: networkTraceTableName, databaseName: "H.Necessaire.Core") { }
        protected override Task<SqlMigration[]> GetAllMigrations() => sqlMigrations.AsTask();
        #endregion

        protected override ISqlFilterCriteria[] ApplyFilter(IDFilter<Guid> filter, DynamicParameters sqlParams)
        {
            List<ISqlFilterCriteria> result = new List<ISqlFilterCriteria>();

            if (filter?.IDs?.Any() ?? false)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(NetworkTrace.ID), parameterName: nameof(filter.IDs), @operator: "IN"));
            }

            return result.ToArray();
        }

        public class NetworkTraceSqlEntry : SqlEntryBase, IGuidIdentity
        {
            public Guid ID { get; set; }
            public Guid? ConsumerIdentityID { get; set; }

            public DateTime AsOf { get; set; }
            public long AsOfTicks { get; set; }

            public string NetworkTraceProviderJson { get; set; }
            public Guid? NetworkTraceProviderID { get; set; }
            public string NetworkTraceProviderIDTag { get; set; }
            public string NetworkTraceProviderDisplayName { get; set; }

            public string IpAddress { get; set; }
            public string NetworkServiceProvider { get; set; }
            public string Organization { get; set; }
            public string ClusterNumber { get; set; }
            public string ClusterName { get; set; }

            public string GeoLocationJson { get; set; }
            public string GeoLocationAddress { get; set; }
            public string GeoLocationGpsPosition { get; set; }
        }

        public class NetworkTraceSqlEntryMapper : SqlEntityMapperBase<NetworkTrace, NetworkTraceSqlEntry>
        {
            static NetworkTraceSqlEntryMapper() => new NetworkTraceSqlEntryMapper().RegisterMapper();

            public override NetworkTrace MapSqlToEntity(NetworkTraceSqlEntry sqlEntity)
            {
                return
                    base.MapSqlToEntity(sqlEntity)
                    .And(x =>
                    {
                        x.AsOf = new DateTime(sqlEntity.AsOfTicks);
                        x.NetworkTraceProvider = sqlEntity.NetworkTraceProviderJson.JsonToObject<InternalIdentity>();
                        x.GeoLocation = sqlEntity.GeoLocationJson.JsonToObject<GeoLocation>();
                    });
            }

            public override NetworkTraceSqlEntry MapEntityToSql(NetworkTrace entity)
            {
                return
                    base.MapEntityToSql(entity)
                    .And(x =>
                    {
                        x.AsOfTicks = entity.AsOf.Ticks;
                        x.NetworkTraceProviderJson = entity.NetworkTraceProvider.ToJsonObject();
                        x.NetworkTraceProviderID = entity.NetworkTraceProvider?.ID;
                        x.NetworkTraceProviderIDTag = entity.NetworkTraceProvider?.IDTag;
                        x.NetworkTraceProviderDisplayName = entity.NetworkTraceProvider?.DisplayName;
                        x.GeoLocationJson = entity.GeoLocation.ToJsonObject();
                        x.GeoLocationAddress = entity.GeoLocation?.Address?.ToString();
                        x.GeoLocationGpsPosition = entity.GeoLocation?.GpsPosition?.ToString();
                    })
                    ;
            }
        }
    }
}
