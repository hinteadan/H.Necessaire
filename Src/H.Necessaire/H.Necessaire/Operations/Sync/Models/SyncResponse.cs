using System;

namespace H.Necessaire
{
    public class SyncResponse : IStringIdentity
    {
        public string ID { get; set; } = Guid.NewGuid().ToString();

        public string PayloadIdentifier { get; set; }

        public string PayloadType { get; set; }

        public DateTime HappenedAt { get; set; } = DateTime.UtcNow;

        public SyncStatus SyncStatus { get; set; } = SyncStatus.NotSynced;
    }
}
