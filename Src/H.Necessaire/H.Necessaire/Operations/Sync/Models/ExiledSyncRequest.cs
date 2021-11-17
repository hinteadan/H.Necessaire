using System;

namespace H.Necessaire
{
    public class ExiledSyncRequest : IStringIdentity
    {
        public string ID => SyncRequest?.ID ?? Guid.Empty.ToString();

        public string Payload => SyncRequest?.Payload;

        public string PayloadIdentifier => SyncRequest?.PayloadIdentifier ?? Guid.Empty.ToString();

        public string PayloadType => SyncRequest?.PayloadType ?? Guid.Empty.ToString();

        public DateTime HappenedAt { get; set; } = DateTime.UtcNow;

        public SyncRequest SyncRequest { get; set; }

        public OperationResult SyncRequestProcessingResult { get; set; }

        public override string ToString()
        {
            return ID;
        }
    }
}
