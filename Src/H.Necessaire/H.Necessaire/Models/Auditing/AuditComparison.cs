using System;
using System.Linq;

namespace H.Necessaire
{
    public class AuditComparison<T>
    {
        public Guid AuditedObjectID { get; set; }
        public string AuditedObjectType { get; set; }

        public Guid PrecedingAuditID { get; set; }
        public Guid FollowingAuditID { get; set; }

        public DateTime PrecedingHappenedAt { get; set; }
        public DateTime FollowingHappenedAt { get; set; }

        public IDentity PrecedingDoneBy { get; set; }
        public IDentity FollowingDoneBy { get; set; }

        public AuditActionType PrecedingActionType { get; set; }
        public AuditActionType FollowingActionType { get; set; }

        public T PrecedingSnapshot { get; set; }
        public T FollowingSnapshot { get; set; }

        public AuditObjectDifference[] Differences { get; set; } = new AuditObjectDifference[0];

        public bool AreEqual => !Differences?.Any() ?? true;
    }
}
