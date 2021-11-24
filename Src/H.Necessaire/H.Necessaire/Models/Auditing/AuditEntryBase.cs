using System;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public abstract class AuditEntryBase : ImAnAuditEntry
    {
        protected AuditEntryBase(ImAnAuditEntry auditEntryMetadata = null)
        {
            ID = auditEntryMetadata?.ID ?? Guid.NewGuid();
            AuditedObjectType = auditEntryMetadata?.AuditedObjectType;
            AuditedObjectID = auditEntryMetadata?.AuditedObjectID;
            HappenedAt = auditEntryMetadata?.HappenedAt ?? DateTime.UtcNow;
            DoneBy = auditEntryMetadata?.DoneBy;
            ActionType = auditEntryMetadata?.ActionType ?? AuditActionType.Modify;
            AppVersion = auditEntryMetadata?.AppVersion;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public string AuditedObjectType { get; set; }
        public string AuditedObjectID { get; set; }
        public DateTime HappenedAt { get; set; } = DateTime.UtcNow;
        public IDentity DoneBy { get; set; }
        public AuditActionType ActionType { get; set; } = AuditActionType.Modify;
        public Version AppVersion { get; set; } = null;

        public abstract Task<T> GetObjectSnapshot<T>();
    }
}
