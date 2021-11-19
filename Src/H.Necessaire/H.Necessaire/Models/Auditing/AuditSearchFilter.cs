using System;

namespace H.Necessaire
{
    public class AuditSearchFilter : IDFilter<Guid>
    {
        public string[] AuditedObjectTypes { get; set; }
        public string[] AuditedObjectIDs { get; set; }
        public AuditActionType[] ActionTypes { get; set; }
        public AuditSearchDoneByFilter DoneBy { get; set; }
        public DateTime? FromInclusive { get; set; }
        public DateTime? ToInclusive { get; set; }

        protected override string[] ValidSortNames { get; } = new string[] { nameof(ImAnAuditEntry.ID), nameof(ImAnAuditEntry.ActionType), nameof(ImAnAuditEntry.AuditedObjectID), nameof(ImAnAuditEntry.AuditedObjectType), nameof(ImAnAuditEntry.HappenedAt) };
    }

    public class AuditSearchDoneByFilter
    {
        public Guid[] IDs { get; set; }
        public string[] IDTags { get; set; }
        public string[] DisplayNames { get; set; }
    }
}
