using System;

namespace H.Necessaire
{
    public class QdActionResultFilter : IDFilter<Guid>
    {
        public Guid[] QdActionIDs { get; set; }
        public DateTime? FromInclusive { get; set; }
        public DateTime? ToInclusive { get; set; }

        protected override string[] ValidSortNames { get; } = new string[] { "ID", "QdActionID", "HappenedAt" };
    }
}
