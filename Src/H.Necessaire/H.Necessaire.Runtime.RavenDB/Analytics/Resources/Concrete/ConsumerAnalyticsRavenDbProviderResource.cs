using H.Necessaire.Analytics;
using H.Necessaire.Runtime.RavenDB.Core.Resources;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Session;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.RavenDB.Analytics.Resources.Concrete
{
    internal class ConsumerAnalyticsRavenDbProviderResource : NetworkTraceRavenDbStorageResource, ImAConsumerAnalyticsProvider
    {
        protected override async Task EnsureIndexes()
        {
            await base.EnsureIndexes();
            await EnsureIndex(() => new ConsumerNetworkTraceIndex());
        }

        public async Task<Page<ConsumerNetworkTrace>> GetConsumerNetworkTraces(ConsumerNetworkTracePageFilter filter = null)
        {
            await EnsureIndexes();

            using (IAsyncDocumentSession session = NewReadSession())
            {
                IAsyncDocumentQuery<ConsumerNetworkTraceWithSearchFields> documentQuery = session.Advanced.AsyncDocumentQuery<ConsumerNetworkTraceWithSearchFields, ConsumerNetworkTraceIndex>();

                long allCount = 0;
                using (IAsyncDocumentSession allCountSession = NewReadSession())
                {
                    allCount = await allCountSession.Advanced.AsyncDocumentQuery<ConsumerNetworkTraceWithSearchFields, ConsumerNetworkTraceIndex>().LongCountAsync();
                }

                documentQuery
                    = ApplyFilter(documentQuery, filter)
                    .ApplyRavenDbSortAndPageFilterIfAny(filter)
                    ;

                ConsumerNetworkTraceWithSearchFields[] pageContent = await documentQuery.ToArrayAsync();

                return Page<ConsumerNetworkTrace>.For(filter, allCount, pageContent);
            }
        }

        public async Task<IDisposableEnumerable<ConsumerNetworkTrace>> StreamConsumerNetworkTraces(ConsumerNetworkTraceFilter filter = null)
        {
            await EnsureIndexes();

            IDocumentSession session = NewSyncReadSession();

            IDocumentQuery<ConsumerNetworkTraceWithSearchFields> documentQuery = session.Advanced.DocumentQuery<ConsumerNetworkTraceWithSearchFields, ConsumerNetworkTraceIndex>();

            ConsumerNetworkTracePageFilter pageFilter = filter.ToPageFilter();

            documentQuery
                = ApplyFilterSync(documentQuery, pageFilter)
                .ApplySyncRavenDbSortAndPageFilterIfAny(pageFilter)
                ;

            return
                new RavenStream<ConsumerNetworkTrace>(session, documentQuery.ToQueryable());
        }

        IAsyncDocumentQuery<ConsumerNetworkTraceWithSearchFields> ApplyFilter(IAsyncDocumentQuery<ConsumerNetworkTraceWithSearchFields> result, ConsumerNetworkTracePageFilter filter)
        {
            if (filter?.ConsumerIdentityIDs?.Any() == true)
            {
                result = result.WhereIn(x => x.ConsumerIdentityID, filter.ConsumerIdentityIDs);
            }

            if (filter?.ConsumerDisplayNames?.Any() == true)
            {
                result = result.Search(x => x.ConsumerDisplayName, string.Join(" ", filter.ConsumerDisplayNames));
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

            if (filter?.IpAddresses?.Any() == true)
            {
                result = result.Search(x => x.IpAddresses, string.Join(" ", filter.IpAddresses));
            }

            if (filter?.NetworkServiceProviders?.Any() == true)
            {
                result = result.Search(x => x.NetworkServiceProviders, string.Join(" ", filter.NetworkServiceProviders));
            }

            if (filter?.Countries?.Any() == true)
            {
                result = result.Search(x => x.GeoLocationAddresses, string.Join(" ", filter.Countries));
            }

            if (filter?.Cities?.Any() == true)
            {
                result = result.Search(x => x.GeoLocationAddresses, string.Join(" ", filter.Cities));
            }

            return result;
        }

        IDocumentQuery<ConsumerNetworkTraceWithSearchFields> ApplyFilterSync(IDocumentQuery<ConsumerNetworkTraceWithSearchFields> result, ConsumerNetworkTracePageFilter filter)
        {
            if (filter?.ConsumerIdentityIDs?.Any() == true)
            {
                result = result.WhereIn(x => x.ConsumerIdentityID, filter.ConsumerIdentityIDs);
            }

            if (filter?.ConsumerDisplayNames?.Any() == true)
            {
                result = result.Search(x => x.ConsumerDisplayName, string.Join(" ", filter.ConsumerDisplayNames));
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

            if (filter?.IpAddresses?.Any() == true)
            {
                result = result.Search(x => x.IpAddresses, string.Join(" ", filter.IpAddresses));
            }

            if (filter?.NetworkServiceProviders?.Any() == true)
            {
                result = result.Search(x => x.NetworkServiceProviders, string.Join(" ", filter.NetworkServiceProviders));
            }

            if (filter?.Countries?.Any() == true)
            {
                result = result.Search(x => x.GeoLocationAddresses, string.Join(" ", filter.Countries));
            }

            if (filter?.Cities?.Any() == true)
            {
                result = result.Search(x => x.GeoLocationAddresses, string.Join(" ", filter.Cities));
            }

            return result;
        }

        public class ConsumerNetworkTraceWithSearchFields : ConsumerNetworkTrace
        {
            public string IpAddresses { get; set; }
            public string NetworkServiceProviders { get; set; }
            public string GeoLocationAddresses { get; set; }
        }

        public class ConsumerNetworkTraceIndex : AbstractIndexCreationTask<NetworkTrace, ConsumerNetworkTraceWithSearchFields>
        {
            public ConsumerNetworkTraceIndex()
            {
                Map = docs => docs.Select(doc =>
                    new ConsumerNetworkTraceWithSearchFields
                    {
                        ConsumerIdentityID = doc.ConsumerIdentityID,
                        ConsumerDisplayName = LoadDocument<ConsumerIdentity>(doc.ConsumerIdentityID.Value.ToString()).DisplayName,

                        LatestVisit = doc.AsOf,
                        OldestVisit = doc.AsOf,

                        IpAddresses = doc.IpAddress,
                        NetworkServiceProviders = doc.NetworkServiceProvider,
                        GeoLocationAddresses = doc.GeoLocation.Address.City.Value.Name + " " + doc.GeoLocation.Address.City.Value.Code + " " + doc.GeoLocation.Address.Country.Value.Name + " " + doc.GeoLocation.Address.Country.Value.Code,

                        NetworkTraces = new ConsumerNetworkTraceEntry[] {
                            new ConsumerNetworkTraceEntry
                            {
                                ID = doc.ID,
                                AsOf = doc.AsOf,
                                IpAddress = doc.IpAddress,
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
                    .GroupBy(x => x.ConsumerIdentityID)
                    .Select(agg =>
                        new ConsumerNetworkTraceWithSearchFields
                        {
                            ConsumerIdentityID = agg.Key,

                            ConsumerDisplayName = agg.First().ConsumerDisplayName,

                            LatestVisit = agg.Max(x => x.LatestVisit),
                            OldestVisit = agg.Min(x => x.OldestVisit),

                            IpAddresses = agg.Select(x => x.IpAddresses).Distinct().Aggregate((a, b) => a + "," + b),
                            NetworkServiceProviders = agg.Select(x => x.NetworkServiceProviders).Distinct().Aggregate((a, b) => a + "," + b),
                            GeoLocationAddresses = agg.Select(x => x.GeoLocationAddresses).Distinct().Aggregate((a, b) => a + "," + b),

                            NetworkTraces = agg.SelectMany(x => x.NetworkTraces).GroupBy(x => x.ID).Select(x => x.First()).OrderByDescending(x => x.AsOf).ToArray()
                        }
                    );
            }
        }
    }
}
