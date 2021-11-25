using System;

namespace H.Necessaire
{
    public class NetworkTrace : IGuidIdentity
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public DateTime AsOf { get; set; } = DateTime.UtcNow;
        public InternalIdentity NetworkTraceProvider { get; set; }

        public string IpAddress { get; set; }
        public string NetworkServiceProvider { get; set; }
        public string Organization { get; set; }
        public string ClusterNumber { get; set; }
        public string ClusterName { get; set; }
        public GeoLocation GeoLocation { get; set; }
    }
}
