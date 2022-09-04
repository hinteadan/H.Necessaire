using H.Necessaire.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Analytics.Resources.Concrete
{
    internal class ConsumerAnalyticsFileSystemProviderResource : ImADependency, ImAConsumerAnalyticsProvider
    {
        #region Construct
        ImAStorageBrowserService<NetworkTrace, IDFilter<Guid>> networkTraceBrowser;
        ImAStorageBrowserService<ConsumerIdentity, IDFilter<Guid>> consumerBrowser;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            networkTraceBrowser = dependencyProvider.Get<ImAStorageBrowserService<NetworkTrace, IDFilter<Guid>>>();
            consumerBrowser = dependencyProvider.Get<ImAStorageBrowserService<ConsumerIdentity, IDFilter<Guid>>>();
        }
        #endregion

        public async Task<Page<ConsumerNetworkTrace>> GetConsumerNetworkTraces(ConsumerNetworkTracePageFilter filter = null)
        {
            using (IDisposableEnumerable<ConsumerIdentity> consumerStream = (await consumerBrowser.StreamAll()).ThrowOnFailOrReturn())
            using (IDisposableEnumerable<NetworkTrace> traceStream = (await networkTraceBrowser.StreamAll()).ThrowOnFailOrReturn())
            {
                Dictionary<Guid, ConsumerIdentity> consumers = consumerStream.ToDictionary(x => x.ID, x => x);

                IEnumerable<ConsumerNetworkTrace> aggregatedStream = AggregateIpAddressNetworkTrace(traceStream, consumers);

                IEnumerable<ConsumerNetworkTrace> resultStream = ApplyPageFilter(aggregatedStream, filter);

                return Page<ConsumerNetworkTrace>.For(filter, aggregatedStream.LongCount(), resultStream.ToArray());
            }
        }

        public async Task<IDisposableEnumerable<ConsumerNetworkTrace>> StreamConsumerNetworkTraces(ConsumerNetworkTraceFilter filter = null)
        {
            using (IDisposableEnumerable<ConsumerIdentity> consumerStream = (await consumerBrowser.StreamAll()).ThrowOnFailOrReturn())
            using (IDisposableEnumerable<NetworkTrace> traceStream = (await networkTraceBrowser.StreamAll()).ThrowOnFailOrReturn())
            {
                Dictionary<Guid, ConsumerIdentity> consumers = consumerStream.ToDictionary(x => x.ID, x => x);

                IEnumerable<ConsumerNetworkTrace> aggregatedStream = AggregateIpAddressNetworkTrace(traceStream, consumers);

                IEnumerable<ConsumerNetworkTrace> resultStream = ApplyStreamFilter(aggregatedStream, filter);

                return resultStream.ToDisposableEnumerable();
            }
        }

        private static IEnumerable<ConsumerNetworkTrace> ApplyPageFilter(IEnumerable<ConsumerNetworkTrace> stream, ConsumerNetworkTracePageFilter filter = null)
        {
            if (filter == null)
                return stream;

            IEnumerable<ConsumerNetworkTrace> resultStream = ApplyBaseFilter(stream, filter);

            resultStream = resultStream.ApplySortAndPageFilterIfAny(filter);

            return resultStream;
        }

        private static IEnumerable<ConsumerNetworkTrace> ApplyStreamFilter(IEnumerable<ConsumerNetworkTrace> stream, ConsumerNetworkTraceFilter filter = null)
        {
            if (filter == null)
                return stream;

            IEnumerable<ConsumerNetworkTrace> resultStream = ApplyBaseFilter(stream, filter);

            resultStream = resultStream.ApplySortFilterIfAny(filter);

            return resultStream;
        }

        private static IEnumerable<ConsumerNetworkTrace> ApplyBaseFilter(IEnumerable<ConsumerNetworkTrace> stream, ConsumerNetworkTraceFilter filter = null)
        {
            if (filter == null)
                return stream;

            IEnumerable<ConsumerNetworkTrace> resultStream = stream;

            if (filter?.ConsumerIdentityIDs?.Any() == true)
                resultStream = resultStream.Where(x => x.ConsumerIdentityID.In(filter.ConsumerIdentityIDs));

            if (filter?.ConsumerDisplayNames?.Any() == true)
                resultStream = resultStream.Where(x => x.ConsumerDisplayName.In(filter.ConsumerDisplayNames, (item, key) => (item?.IndexOf(key, StringComparison.InvariantCultureIgnoreCase) ?? -1) >= 0));

            if (filter?.FromInclusive != null)
                resultStream = resultStream.Where(x => x.LatestVisit >= filter.FromInclusive.Value || x.OldestVisit >= filter.FromInclusive.Value);

            if (filter?.ToInclusive != null)
                resultStream = resultStream.Where(x => x.LatestVisit <= filter.ToInclusive.Value || x.OldestVisit <= filter.ToInclusive.Value);

            if (filter?.IpAddresses?.Any() == true)
                resultStream = resultStream.Where(t => t.NetworkTraces.Any(x => x.IpAddress.In(filter.IpAddresses, (item, key) => string.Equals(item, key, StringComparison.InvariantCultureIgnoreCase))));

            if (filter?.Countries?.Any() == true)
                resultStream
                    = resultStream
                    .Where(t =>
                        t.NetworkTraces.Any(x => (x.Location?.CountryName?.In(filter.Countries, (item, key) => (item?.IndexOf(key, StringComparison.InvariantCultureIgnoreCase) ?? -1) >= 0) == true))

                        ||
                        t.NetworkTraces.Any(x => (x.Location?.CountryCode?.In(filter.Countries, (item, key) => (item?.IndexOf(key, StringComparison.InvariantCultureIgnoreCase) ?? -1) >= 0) == true))
                    )
                    ;

            if (filter?.Cities?.Any() == true)
                resultStream
                    = resultStream
                    .Where(t =>
                        t.NetworkTraces.Any(x => (x.Location?.CityName?.In(filter.Cities, (item, key) => (item?.IndexOf(key, StringComparison.InvariantCultureIgnoreCase) ?? -1) >= 0) == true))
                    )
                    ;

            return resultStream;
        }

        private IEnumerable<ConsumerNetworkTrace> AggregateIpAddressNetworkTrace(IEnumerable<NetworkTrace> traces, IDictionary<Guid, ConsumerIdentity> consumerDictionary)
        {
            if (traces?.Any() != true)
                return Enumerable.Empty<ConsumerNetworkTrace>();

            return
                traces
                .GroupBy(x => x.ConsumerIdentityID)
                .Select(x => new ConsumerNetworkTrace
                {
                    ConsumerIdentityID = x.Key,
                    ConsumerDisplayName = (x.Key != null && consumerDictionary?.ContainsKey(x.Key.Value) == true) ? consumerDictionary[x.Key.Value]?.DisplayName : null,
                    LatestVisit = x.Max(g => g.AsOf),
                    OldestVisit = x.Min(g => g.AsOf),
                    NetworkTraces = x.Select(
                        g => new ConsumerNetworkTraceEntry
                        {
                            AsOf = g.AsOf,
                            NetworkServiceProvider = g.NetworkServiceProvider,
                            IpAddress = g.IpAddress,
                            Location = new NetworkTraceGeoLocation
                            {
                                CityName = g.GeoLocation?.Address?.City?.ToString(),
                                CountryCode = g.GeoLocation?.Address?.Country?.Code,
                                CountryName = g.GeoLocation?.Address?.Country?.Name,
                            },
                        }.And(t =>
                        {
                            t.LocationLabel = t.Location?.ToString();
                        })
                    )
                    .OrderByDescending(g => g.AsOf)
                    .ToArray(),
                });
        }
    }
}
