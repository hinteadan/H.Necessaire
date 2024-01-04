using H.Necessaire.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Analytics.Resources
{
    internal class NetworkAddressAnalyticsGoogleFirestoreDbProviderResource
        : ImADependency, ImANetworkAddressAnalyticsProvider
    {
        #region Construct
        ImAStorageBrowserService<NetworkTrace, IDFilter<Guid>> networkTraceBrowser;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            networkTraceBrowser = dependencyProvider.Get<ImAStorageBrowserService<NetworkTrace, IDFilter<Guid>>>();
        }
        #endregion

        public async Task<Page<IpAddressNetworkTrace>> GetIpAddressNetworkTraces(IpAddressNetworkTracePageFilter filter = null)
        {
            using (IDisposableEnumerable<NetworkTrace> traceStream = (await networkTraceBrowser.StreamAll()).ThrowOnFailOrReturn())
            {
                IEnumerable<IpAddressNetworkTrace> aggregatedStream = AggregateIpAddressNetworkTrace(traceStream);

                IEnumerable<IpAddressNetworkTrace> resultStream = ApplyPageFilter(aggregatedStream, filter);

                return Page<IpAddressNetworkTrace>.For(filter, aggregatedStream.LongCount(), resultStream.ToArray());
            }
        }
        public async Task<IDisposableEnumerable<IpAddressNetworkTrace>> StreamIpAddressNetworkTraces(IpAddressNetworkTraceFilter filter = null)
        {
            using (IDisposableEnumerable<NetworkTrace> traceStream = (await networkTraceBrowser.StreamAll()).ThrowOnFailOrReturn())
            {
                IEnumerable<IpAddressNetworkTrace> aggregatedStream = AggregateIpAddressNetworkTrace(traceStream);

                IEnumerable<IpAddressNetworkTrace> resultStream = ApplyStreamFilter(aggregatedStream, filter);

                return resultStream.ToDisposableEnumerable();
            }
        }

        private static IEnumerable<IpAddressNetworkTrace> ApplyPageFilter(IEnumerable<IpAddressNetworkTrace> stream, IpAddressNetworkTracePageFilter filter = null)
        {
            if (filter == null)
                return stream;

            IEnumerable<IpAddressNetworkTrace> resultStream = ApplyBaseFilter(stream, filter);

            resultStream = resultStream.ApplySortAndPageFilterIfAny(filter);

            return resultStream;
        }

        private static IEnumerable<IpAddressNetworkTrace> ApplyStreamFilter(IEnumerable<IpAddressNetworkTrace> stream, IpAddressNetworkTraceFilter filter = null)
        {
            if (filter == null)
                return stream;

            IEnumerable<IpAddressNetworkTrace> resultStream = ApplyBaseFilter(stream, filter);

            resultStream = resultStream.ApplySortFilterIfAny(filter);

            return resultStream;
        }

        private static IEnumerable<IpAddressNetworkTrace> ApplyBaseFilter(IEnumerable<IpAddressNetworkTrace> stream, IpAddressNetworkTraceFilter filter = null)
        {
            if (filter == null)
                return stream;

            IEnumerable<IpAddressNetworkTrace> resultStream = stream;

            if (filter?.IpAddresses?.Any() == true)
                resultStream = resultStream.Where(x => x.IpAddress.In(filter.IpAddresses, (item, key) => string.Equals(item, key, StringComparison.InvariantCultureIgnoreCase)));

            if (filter?.FromInclusive != null)
                resultStream = resultStream.Where(x => x.LatestVisit >= filter.FromInclusive.Value || x.OldestVisit >= filter.FromInclusive.Value);

            if (filter?.ToInclusive != null)
                resultStream = resultStream.Where(x => x.LatestVisit <= filter.ToInclusive.Value || x.OldestVisit <= filter.ToInclusive.Value);

            if (filter?.NetworkServiceProviders?.Any() == true)
                resultStream = resultStream.Where(t => t.NetworkTraces.Any(x => x.NetworkServiceProvider.In(filter.NetworkServiceProviders, (item, key) => (item?.IndexOf(key, StringComparison.InvariantCultureIgnoreCase) ?? -1) >= 0)));

            if (filter?.ConsumerIdentityIDs?.Any() == true)
                resultStream = resultStream.Where(t => t.NetworkTraces.Any(x => x.ConsumerIdentityID.In(filter.ConsumerIdentityIDs)));

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

        private static IEnumerable<IpAddressNetworkTrace> AggregateIpAddressNetworkTrace(IEnumerable<NetworkTrace> traces)
        {
            if (traces?.Any() != true)
                return Enumerable.Empty<IpAddressNetworkTrace>();

            return
                traces
                .GroupBy(x => x.IpAddress)
                .Select(x => new IpAddressNetworkTrace
                {
                    IpAddress = x.Key,
                    LatestVisit = x.Max(g => g.AsOf),
                    OldestVisit = x.Min(g => g.AsOf),
                    NetworkTraces = x.Select(
                        g => new IpAddressNetworkTraceEntry
                        {
                            AsOf = g.AsOf,
                            ConsumerIdentityID = g.ConsumerIdentityID,
                            NetworkServiceProvider = g.NetworkServiceProvider,
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
