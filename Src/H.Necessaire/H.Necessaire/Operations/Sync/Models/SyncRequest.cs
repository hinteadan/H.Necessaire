using System;

namespace H.Necessaire
{
    public class SyncRequest : IStringIdentity
    {
        public string ID => $"{PayloadType}_{PayloadIdentifier}";

        public string Payload { get; set; }

        public string PayloadIdentifier { get; set; }

        public string PayloadType { get; set; }

        public SyncStatus SyncStatus { get; set; } = SyncStatus.NotSynced;

        public DateTime HappenedAt { get; set; } = DateTime.UtcNow;

        public OperationContext OperationContext { get; set; }

        public override string ToString()
        {
            return ID;
        }
    }
}
