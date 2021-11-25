using System;

namespace H.Necessaire
{
    public class QdActionFilter : IDFilter<Guid>
    {
        public string[] Types { get; set; }
        public QdActionStatus[] Statuses { get; set; }
        public int? MinRunCount { get; set; }
        public int? MaxRunCount { get; set; }
        public DateTime? FromInclusive { get; set; }
        public DateTime? ToInclusive { get; set; }

        protected override string[] ValidSortNames { get; } = new string[] { "ID", "RunCount", "Status", "Type", "QdAt" };
    }
}
