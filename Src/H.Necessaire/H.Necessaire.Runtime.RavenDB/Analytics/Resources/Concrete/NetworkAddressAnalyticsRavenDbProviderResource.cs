using H.Necessaire.Analytics;
using H.Necessaire.Runtime.RavenDB.Core.Resources;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Session;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.RavenDB.Analytics.Resources.Concrete
{
    internal class NetworkAddressAnalyticsRavenDbProviderResource : NetworkTraceRavenDbStorageResource, ImANetworkAddressAnalyticsProvider
    {
        protected override async Task EnsureIndexes()
        {
            await base.EnsureIndexes();
            await EnsureIndex(() => new IpAddressNetworkTraceIndex());
        }

        public async Task<Page<IpAddressNetworkTrace>> GetIpAddressNetworkTraces(IpAddressNetworkTracePageFilter filter = null)
        {
            await EnsureIndexes();

            using (IAsyncDocumentSession session = NewReadSession())
            {
                IAsyncDocumentQuery<IpAddressNetworkTraceWithSearchFields> documentQuery = session.Advanced.AsyncDocumentQuery<IpAddressNetworkTraceWithSearchFields, IpAddressNetworkTraceIndex>();

                long allCount = 0;
                using (IAsyncDocumentSession allCountSession = NewReadSession())
                {
                    allCount = await allCountSession.Advanced.AsyncDocumentQuery<IpAddressNetworkTraceWithSearchFields, IpAddressNetworkTraceIndex>().LongCountAsync();
                }

                documentQuery
                    = ApplyFilter(documentQuery, filter)
                    .ApplyRavenDbSortAndPageFilterIfAny(filter)
                    ;

                IpAddressNetworkTraceWithSearchFields[] pageContent = await documentQuery.ToArrayAsync();

                return Page<IpAddressNetworkTrace>.For(filter, allCount, pageContent);
            }
        }

        public async Task<IDisposableEnumerable<IpAddressNetworkTrace>> StreamIpAddressNetworkTraces(IpAddressNetworkTraceFilter filter = null)
        {
            await EnsureIndexes();

            IDocumentSession session = NewSyncReadSession();

            IDocumentQuery<IpAddressNetworkTraceWithSearchFields> documentQuery = session.Advanced.DocumentQuery<IpAddressNetworkTraceWithSearchFields, IpAddressNetworkTraceIndex>();

            IpAddressNetworkTracePageFilter pageFilter = filter.ToPageFilter();

            documentQuery
                = ApplyFilterSync(documentQuery, pageFilter)
                .ApplySyncRavenDbSortAndPageFilterIfAny(pageFilter)
                ;

            return
                new RavenStream<IpAddressNetworkTrace>(session, documentQuery.ToQueryable());
        }

        IAsyncDocumentQuery<IpAddressNetworkTraceWithSearchFields> ApplyFilter(IAsyncDocumentQuery<IpAddressNetworkTraceWithSearchFields> result, IpAddressNetworkTracePageFilter filter)
        {
            if (filter?.IpAddresses?.Any() == true)
            {
                result = result.WhereIn(x => x.IpAddress, filter.IpAddresses);
            }

            if (filter?.FromInclusive != null)
            {
                result = result
                    .OpenSubclause()
                    .UsingDefaultOperator(QueryOperator.Or)
                    .WhereGreaterThanOrEqual(x => x.OldestVisit, filter.FromInclusive.Value, exact: true)
                    .WhereGreaterThanOrEqual(x => x.LatestVisit, filter.FromInclusive.Value, exact: true)
                    .CloseSubclause()
                    ;
            }

            if (filter?.ToInclusive != null)
            {
                result = result
                    .OpenSubclause()
                    .UsingDefaultOperator(QueryOperator.Or)
                    .WhereLessThanOrEqual(x => x.OldestVisit, filter.ToInclusive.Value, exact: true)
                    .WhereLessThanOrEqual(x => x.LatestVisit, filter.ToInclusive.Value, exact: true)
                    .CloseSubclause()
                    ;
            }

            if (filter?.NetworkServiceProviders?.Any() == true)
            {
                result = result.WhereIn(x => x.NetworkServiceProviders, filter.NetworkServiceProviders);
            }

            if (filter?.ConsumerIdentityIDs?.Any() == true)
            {
                result = result.WhereIn(x => x.ConsumerIdentityIDs, filter.ConsumerIdentityIDs.ToStringArray());
            }

            if (filter?.Countries?.Any() == true)
            {
                result = result.WhereIn(x => x.GeoLocationAddresses, filter.Countries);
            }

            if (filter?.Cities?.Any() == true)
            {
                result = result.WhereIn(x => x.GeoLocationAddresses, filter.Cities);
            }

            return result;
        }

        IDocumentQuery<IpAddressNetworkTraceWithSearchFields> ApplyFilterSync(IDocumentQuery<IpAddressNetworkTraceWithSearchFields> result, IpAddressNetworkTracePageFilter filter)
        {
            if (filter?.IpAddresses?.Any() == true)
            {
                result = result.WhereIn(x => x.IpAddress, filter.IpAddresses);
            }

            if (filter?.FromInclusive != null)
            {
                result = result
                    .OpenSubclause()
                    .UsingDefaultOperator(QueryOperator.Or)
                    .WhereGreaterThanOrEqual(x => x.OldestVisit, filter.FromInclusive.Value, exact: true)
                    .WhereGreaterThanOrEqual(x => x.LatestVisit, filter.FromInclusive.Value, exact: true)
                    .CloseSubclause()
                    ;
            }

            if (filter?.ToInclusive != null)
            {
                result = result
                    .OpenSubclause()
                    .UsingDefaultOperator(QueryOperator.Or)
                    .WhereLessThanOrEqual(x => x.OldestVisit, filter.ToInclusive.Value, exact: true)
                    .WhereLessThanOrEqual(x => x.LatestVisit, filter.ToInclusive.Value, exact: true)
                    .CloseSubclause()
                    ;
            }

            if (filter?.NetworkServiceProviders?.Any() == true)
            {
                result = result.WhereIn(x => x.NetworkServiceProviders, filter.NetworkServiceProviders);
            }

            if (filter?.ConsumerIdentityIDs?.Any() == true)
            {
                result = result.WhereIn(x => x.ConsumerIdentityIDs, filter.ConsumerIdentityIDs.ToStringArray());
            }

            if (filter?.Countries?.Any() == true)
            {
                result = result.WhereIn(x => x.GeoLocationAddresses, filter.Countries);
            }

            if (filter?.Cities?.Any() == true)
            {
                result = result.WhereIn(x => x.GeoLocationAddresses, filter.Cities);
            }

            return result;
        }

        public class IpAddressNetworkTraceWithSearchFields : IpAddressNetworkTrace
        {
            public string ConsumerIdentityIDs { get; set; }
            public string NetworkServiceProviders { get; set; }
            public string GeoLocationAddresses { get; set; }
        }

        public class IpAddressNetworkTraceIndex : AbstractIndexCreationTask<NetworkTrace, IpAddressNetworkTraceWithSearchFields>
        {
            public IpAddressNetworkTraceIndex()
            {
                Map = docs => docs.Select(doc =>
                    new IpAddressNetworkTraceWithSearchFields
                    {
                        IpAddress = doc.IpAddress,

                        LatestVisit = doc.AsOf,
                        OldestVisit = doc.AsOf,

                        ConsumerIdentityIDs = doc.ConsumerIdentityID.ToString(),
                        NetworkServiceProviders = doc.NetworkServiceProvider,
                        GeoLocationAddresses = doc.GeoLocation.Address.City.Value.Name + " " + doc.GeoLocation.Address.City.Value.Code + " " + doc.GeoLocation.Address.Country.Value.Name + " " + doc.GeoLocation.Address.Country.Value.Code,

                        NetworkTraces = new IpAddressNetworkTraceEntry[] {
                            new IpAddressNetworkTraceEntry
                            {
                                ID = doc.ID,
                                AsOf = doc.AsOf,
                                ConsumerIdentityID = doc.ConsumerIdentityID,
                                NetworkServiceProvider = doc.NetworkServiceProvider,
                                LocationLabel = doc.GeoLocation.Address.City.Value.Name + ", " + doc.GeoLocation.Address.Country.Value.Name + " (" + doc.GeoLocation.Address.Country.Value.Code + ")",
                                Location = new NetworkTraceGeoLocation
                                {
                                    CityName = doc.GeoLocation.Address.City.Value.Name,
                                    CountryCode = doc.GeoLocation.Address.Country.Value.Code,
                                    CountryName = doc.GeoLocation.Address.Country.Value.Name,
                                },
                            }
                        },
                    }
                );

                Reduce = res => res
                    .GroupBy(x => x.IpAddress)
                    .Select(agg =>
                        new IpAddressNetworkTraceWithSearchFields
                        {
                            IpAddress = agg.Key,

                            LatestVisit = agg.Max(x => x.LatestVisit),
                            OldestVisit = agg.Min(x => x.OldestVisit),

                            ConsumerIdentityIDs = agg.Select(x => x.ConsumerIdentityIDs).Distinct().Aggregate((a, b) => a + "," + b),
                            NetworkServiceProviders = agg.Select(x => x.NetworkServiceProviders).Distinct().Aggregate((a, b) => a + "," + b),
                            GeoLocationAddresses = agg.Select(x => x.GeoLocationAddresses).Distinct().Aggregate((a, b) => a + "," + b),

                            NetworkTraces = agg.SelectMany(x => x.NetworkTraces).GroupBy(x => x.ID).Select(x => x.First()).OrderByDescending(x => x.AsOf).ToArray()
                        }
                    );
            }
        }
    }
}
