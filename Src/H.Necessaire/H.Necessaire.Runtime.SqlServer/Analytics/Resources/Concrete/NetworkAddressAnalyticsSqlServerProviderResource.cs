using Dapper;
using H.Necessaire.Analytics;
using H.Necessaire.Dapper;
using H.Necessaire.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.SqlServer.Analytics.Resources.Concrete
{
    internal partial class NetworkAddressAnalyticsSqlServerProviderResource : DapperStorageServiceBase<string, IpAddressNetworkTrace, NetworkAddressAnalyticsSqlServerProviderResource.IpAddressNetworkTraceSqlEntry, IpAddressNetworkTracePageFilter>, ImANetworkAddressAnalyticsProvider
    {
        #region Construct
        public NetworkAddressAnalyticsSqlServerProviderResource() : base(connectionString: null, tableName: networkAddressAnalyticsViewName, databaseName: "H.Necessaire.Core") { }
        protected override Task<SqlMigration[]> GetAllMigrations() => sqlMigrations.AsTask();

        protected override ISqlFilterCriteria[] ApplyFilter(IpAddressNetworkTracePageFilter filter, DynamicParameters sqlParams)
        {
            List<ISqlFilterCriteria> result = new List<ISqlFilterCriteria>();

            if (filter?.IpAddresses?.Any() == true)
            {
                result.Add(new ComposedSqlFilterCriteria(
                    filter
                    .IpAddresses
                    .Select(
                        (searchKey, index) =>
                        {
                            string paramName = $"{nameof(IpAddressNetworkTraceSqlEntry.IpAddress)}{index}";
                            sqlParams.Add(paramName, searchKey);
                            return
                                new SqlFilterCriteria(
                                    columnName: nameof(IpAddressNetworkTraceSqlEntry.IpAddress),
                                    parameterName: paramName,
                                    @operator: "like"
                                )
                                .And(x => x.LikeOperatorMatch = SqlFilterCriteria.LikeOperatorMatchType.Exact)
                                ;
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

            if (filter?.NetworkServiceProviders?.Any() == true)
            {
                result.Add(new ComposedSqlFilterCriteria(
                    filter
                    .NetworkServiceProviders
                    .Select(
                        (searchKey, index) =>
                        {
                            string paramName = $"{nameof(IpAddressNetworkTraceSqlEntry.NetworkServiceProviders)}{index}";
                            sqlParams.Add(paramName, searchKey ?? "null");
                            return
                                new SqlFilterCriteria(
                                    columnName: nameof(IpAddressNetworkTraceSqlEntry.NetworkServiceProviders),
                                    parameterName: paramName,
                                    @operator: "like"
                                );
                        }
                    )
                    .ToArray()
                ));
            }

            if (filter?.ConsumerIdentityIDs?.Any() == true)
            {
                result.Add(new ComposedSqlFilterCriteria(
                    filter
                    .ConsumerIdentityIDs
                    .Select(
                        (searchKey, index) =>
                        {
                            string paramName = $"{nameof(IpAddressNetworkTraceSqlEntry.ConsumerIdentityIDs)}{index}";
                            sqlParams.Add(paramName, searchKey?.ToString() ?? "null");
                            return
                                new SqlFilterCriteria(
                                    columnName: nameof(IpAddressNetworkTraceSqlEntry.ConsumerIdentityIDs),
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
                            string paramName = $"{nameof(IpAddressNetworkTraceSqlEntry.GeoLocationAddresses)}Country{index}";
                            sqlParams.Add(paramName, searchKey?.ToString() ?? "null");
                            return
                                new SqlFilterCriteria(
                                    columnName: nameof(IpAddressNetworkTraceSqlEntry.GeoLocationAddresses),
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
                            string paramName = $"{nameof(IpAddressNetworkTraceSqlEntry.GeoLocationAddresses)}City{index}";
                            sqlParams.Add(paramName, searchKey?.ToString() ?? "null");
                            return
                                new SqlFilterCriteria(
                                    columnName: nameof(IpAddressNetworkTraceSqlEntry.GeoLocationAddresses),
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

        public async Task<Page<IpAddressNetworkTrace>> GetIpAddressNetworkTraces(IpAddressNetworkTracePageFilter filter = null)
        {
            return (await LoadPage(filter))?.ThrowOnFailOrReturn() ?? Page<IpAddressNetworkTrace>.Empty();
        }

        public async Task<IDisposableEnumerable<IpAddressNetworkTrace>> StreamIpAddressNetworkTraces(IpAddressNetworkTraceFilter filter = null)
        {
            return (await Stream(filter?.ToPageFilter()))?.ThrowOnFailOrReturn() ?? (Enumerable.Empty<IpAddressNetworkTrace>().ToDisposableEnumerable());
        }


        public class IpAddressNetworkTraceSqlEntry : SqlEntryBase
        {
            public string IpAddress { get; set; }
            public long LatestVisitTicks { get; set; }
            public long OldestVisitTicks { get; set; }
            public string ConsumerIdentityIDs { get; set; }
            public string NetworkServiceProviders { get; set; }
            public string GeoLocationAddresses { get; set; }
            public string NetworkTracesJson { get; set; }
        }

        public class IpAddressNetworkTraceSqlEntryMapper : SqlEntityMapperBase<IpAddressNetworkTrace, IpAddressNetworkTraceSqlEntry>
        {
            static IpAddressNetworkTraceSqlEntryMapper() => new IpAddressNetworkTraceSqlEntryMapper().RegisterMapper();

            public override IpAddressNetworkTrace MapSqlToEntity(IpAddressNetworkTraceSqlEntry sqlEntity)
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
                            ?.Select(t => new IpAddressNetworkTraceEntry
                            {
                                ID = t.ID,
                                AsOf = new DateTime(t.AsOfTicks),
                                ConsumerIdentityID = t.ConsumerIdentityID,
                                NetworkServiceProvider = t.NetworkServiceProvider,
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
                public Guid? ConsumerIdentityID { get; set; }
                public string NetworkServiceProvider { get; set; }
                public string LocationLabel { get; set; }
                public GeoLocation Location { get; set; }
            }
        }
    }
}
