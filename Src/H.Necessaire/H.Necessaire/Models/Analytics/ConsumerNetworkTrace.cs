using System;

namespace H.Necessaire.Analytics
{
    public class ConsumerNetworkTrace : IGuidIdentity
    {
        public Guid ID => ConsumerIdentityID ?? Guid.Empty;
        public Guid? ConsumerIdentityID { get; set; }
        public string ConsumerDisplayName { get; set; }
        public DateTime LatestVisit { get; set; }
        public DateTime OldestVisit { get; set; }
        public TimeSpan Age => LatestVisit - OldestVisit;
        public ConsumerNetworkTraceEntry[] NetworkTraces { get; set; }
    }

    public class ConsumerNetworkTraceEntry
    {
        public Guid ID { get; set; }
        public string IpAddress { get; set; }
        public string NetworkServiceProvider { get; set; }
        public DateTime AsOf { get; set; }
        public string LocationLabel { get; set; }
        public NetworkTraceGeoLocation Location { get; set; }
    }
}
