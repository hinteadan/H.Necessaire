using Dapper;
using H.Necessaire.Analytics;
using H.Necessaire.Dapper;
using H.Necessaire.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static H.Necessaire.Runtime.SqlServer.Analytics.Resources.Concrete.NetworkAddressAnalyticsSqlServerProviderResource;

namespace H.Necessaire.Runtime.SqlServer.Analytics.Resources.Concrete
{
    internal partial class ConsumerAnalyticsSqlServerProviderResource : DapperStorageServiceBase<Guid, ConsumerNetworkTrace, ConsumerAnalyticsSqlServerProviderResource.ConsumerNetworkTraceSqlEntry, ConsumerNetworkTracePageFilter>, ImAConsumerAnalyticsProvider
    {
        #region Construct
        public ConsumerAnalyticsSqlServerProviderResource() : base(connectionString: null, tableName: consumerAnalyticsViewName, databaseName: "H.Necessaire.Core") { }
        protected override Task<SqlMigration[]> GetAllMigrations() => sqlMigrations.AsTask();

        protected override ISqlFilterCriteria[] ApplyFilter(ConsumerNetworkTracePageFilter filter, DynamicParameters sqlParams)
        {
            List<ISqlFilterCriteria> result = new List<ISqlFilterCriteria>();

            if (filter?.ConsumerIdentityIDs?.Any() == true)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(ConsumerNetworkTraceSqlEntry.ConsumerIdentityID), parameterName: nameof(ConsumerNetworkTracePageFilter.ConsumerIdentityIDs), @operator: "IN"));
            }

            if (filter?.ConsumerDisplayNames?.Any() == true)
            {
                result.Add(new ComposedSqlFilterCriteria(
                    filter
                    .ConsumerDisplayNames
                    .Select(
                        (searchKey, index) =>
                        {
                            string paramName = $"{nameof(ConsumerNetworkTraceSqlEntry.ConsumerDisplayName)}{index}";
                            sqlParams.Add(paramName, searchKey);
                            return
                                new SqlFilterCriteria(
                                    columnName: nameof(ConsumerNetworkTraceSqlEntry.ConsumerDisplayName),
                                    parameterName: paramName,
                                    @operator: "like"
                                );
                        }
                    )
                    .ToArray()
                ));
            }

            if (filter?.FromInclusive != null)
            {
                string paramName = $"{nameof(filter.FromInclusive)}Ticks";
                sqlParams.Add(paramName, filter.FromInclusive.Value.Ticks);
                result.Add(new ComposedSqlFilterCriteria(
                        new SqlFilterCriteria(
                            columnName: nameof(IpAddressNetworkTraceSqlEntry.OldestVisitTicks),
                            parameterName: paramName,
                            @operator: ">="
                        ),
                        new SqlFilterCriteria(
                            columnName: nameof(IpAddressNetworkTraceSqlEntry.LatestVisitTicks),
                            parameterName: paramName,
                            @operator: ">="
                        )
                    ));
            }

            if (filter?.ToInclusive != null)
            {
                string paramName = $"{nameof(filter.ToInclusive)}Ticks";
                sqlParams.Add(paramName, filter.ToInclusive.Value.Ticks);
                result.Add(new ComposedSqlFilterCriteria(
                        new SqlFilterCriteria(
                            columnName: nameof(IpAddressNetworkTraceSqlEntry.OldestVisitTicks),
                            parameterName: paramName,
                            @operator: "<="
                        ),
                        new SqlFilterCriteria(
                            columnName: nameof(IpAddressNetworkTraceSqlEntry.LatestVisitTicks),
                            parameterName: paramName,
                            @operator: "<="
                        )
                    ));
            }

            if (filter?.IpAddresses?.Any() == true)
            {
                result.Add(new ComposedSqlFilterCriteria(
                    filter
                    .IpAddresses
                    .Select(
                        (searchKey, index) =>
                        {
                            string paramName = $"{nameof(ConsumerNetworkTraceSqlEntry.IpAddresses)}{index}";
                            sqlParams.Add(paramName, searchKey ?? "null");
                            return
                                new SqlFilterCriteria(
                                    columnName: nameof(ConsumerNetworkTraceSqlEntry.IpAddresses),
                                    parameterName: paramName,
                                    @operator: "like"
                                );
                        }
                    )
                    .ToArray()
                ));

            }

            if (filter?.Countries?.Any() == true)
            {
                result.Add(new ComposedSqlFilterCriteria(
                    filter
                    .Countries
                    .Select(
                        (searchKey, index) =>
                        {
                            string paramName = $"{nameof(ConsumerNetworkTraceSqlEntry.GeoLocationAddresses)}Country{index}";
                            sqlParams.Add(paramName, searchKey?.ToString() ?? "null");
                            return
                                new SqlFilterCriteria(
                                    columnName: nameof(ConsumerNetworkTraceSqlEntry.GeoLocationAddresses),
                                    parameterName: paramName,
                                    @operator: "like"
                                );
                        }
                    )
                    .ToArray()
                ));
            }

            if (filter?.Cities?.Any() == true)
            {
                result.Add(new ComposedSqlFilterCriteria(
                    filter
                    .Cities
                    .Select(
                        (searchKey, index) =>
                        {
                            string paramName = $"{nameof(ConsumerNetworkTraceSqlEntry.GeoLocationAddresses)}City{index}";
                            sqlParams.Add(paramName, searchKey?.ToString() ?? "null");
                            return
                                new SqlFilterCriteria(
                                    columnName: nameof(ConsumerNetworkTraceSqlEntry.GeoLocationAddresses),
                                    parameterName: paramName,
                                    @operator: "like"
                                );
                        }
                    )
                    .ToArray()
                ));
            }

            return result.ToArray();
        }
        #endregion

        public async Task<Page<ConsumerNetworkTrace>> GetConsumerNetworkTraces(ConsumerNetworkTracePageFilter filter = null)
        {
            return (await LoadPage(filter))?.ThrowOnFailOrReturn() ?? Page<ConsumerNetworkTrace>.Empty();
        }

        public async Task<IDisposableEnumerable<ConsumerNetworkTrace>> StreamConsumerNetworkTraces(ConsumerNetworkTraceFilter filter = null)
        {
            return (await Stream(filter?.ToPageFilter()))?.ThrowOnFailOrReturn() ?? (Enumerable.Empty<ConsumerNetworkTrace>().ToDisposableEnumerable());
        }


        public class ConsumerNetworkTraceSqlEntry : SqlEntryBase
        {
            public Guid? ConsumerIdentityID { get; set; }
            public string ConsumerDisplayName { get; set; }
            public long LatestVisitTicks { get; set; }
            public long OldestVisitTicks { get; set; }
            public string IpAddresses { get; set; }
            public string GeoLocationAddresses { get; set; }
            public string NetworkTracesJson { get; set; }
        }

        public class ConsumerNetworkTraceSqlEntryMapper : SqlEntityMapperBase<ConsumerNetworkTrace, ConsumerNetworkTraceSqlEntry>
        {
            static ConsumerNetworkTraceSqlEntryMapper() => new ConsumerNetworkTraceSqlEntryMapper().RegisterMapper();

            public override ConsumerNetworkTrace MapSqlToEntity(ConsumerNetworkTraceSqlEntry sqlEntity)
            {
                return
                    base.MapSqlToEntity(sqlEntity)
                    .And(x =>
                    {
                        x.LatestVisit = new DateTime(sqlEntity.LatestVisitTicks);
                        x.OldestVisit = new DateTime(sqlEntity.OldestVisitTicks);
                        x.NetworkTraces
                            = sqlEntity
                            .NetworkTracesJson
                            .JsonToObject<NetworkTraceJsonEntry[]>()
                            ?.Select(t => new ConsumerNetworkTraceEntry
                            {
                                ID = t.ID,
                                AsOf = new DateTime(t.AsOfTicks),
                                IpAddress = t.IpAddress,
                                LocationLabel = t.LocationLabel,
                                Location = new NetworkTraceGeoLocation
                                {
                                    CityName = t.Location?.Address?.City?.Name,
                                    CountryCode = t.Location?.Address?.Country?.Code,
                                    CountryName = t.Location?.Address?.Country?.Name,
                                }
                            })
                            .ToArray()
                            ;
                    });
            }

            private class NetworkTraceJsonEntry
            {
                public Guid ID { get; set; }
                public long AsOfTicks { get; set; }
                public string IpAddress { get; set; }
                public string LocationLabel { get; set; }
                public GeoLocation Location { get; set; }
            }
        }
    }
}
