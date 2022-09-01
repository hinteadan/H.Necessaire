using System;

namespace H.Necessaire.Analytics
{
    public class IpAddressNetworkTrace : IStringIdentity
    {
        public string ID => IpAddress;
        public string IpAddress { get; set; }
        public DateTime LatestVisit { get; set; }
        public DateTime OldestVisit { get; set; }
        public TimeSpan Age => LatestVisit - OldestVisit;
        public IpAddressNetworkTraceEntry[] NetworkTraces { get; set; }
    }

    public class IpAddressNetworkTraceEntry
    {
        public Guid ID { get; set; }
        public Guid? ConsumerIdentityID { get; set; }
        public string NetworkServiceProvider { get; set; }
        public DateTime AsOf { get; set; }
        public string LocationLabel { get; set; }
        public NetworkTraceGeoLocation Location { get; set; }
    }
}
