using System;

namespace H.Necessaire
{
    public class SyncRequestFilter : SortFilterBase, IPageFilter
    {
        static readonly string[] sortableProps = new string[] { nameof(SyncRequest.ID), nameof(SyncRequest.PayloadType), nameof(SyncRequest.HappenedAt) };

        public string[] IDs { get; set; }
        public string[] PayloadIdentifiers { get; set; }
        public string[] PayloadTypes { get; set; }
        public SyncStatus[] SyncStatuses { get; set; }
        public DateTime? FromInclusive { get; set; }
        public DateTime? ToInclusive { get; set; }

        public PageFilter PageFilter { get; set; }

        protected override string[] ValidSortNames => sortableProps;
    }
}
