using System;

namespace H.Necessaire
{
    public class LogFilter : SortFilterBase, IPageFilter
    {
        public Guid[] IDs { get; set; }
        public LogEntryLevel[] Levels { get; set; }
        public Guid[] ScopeIDs { get; set; }
        public string[] Methods { get; set; }
        public string[] Components { get; set; }
        public string[] Applications { get; set; }
        public DateTime? FromInclusive { get; set; }
        public DateTime? ToInclusive { get; set; }

        public PageFilter PageFilter { get; set; }

        protected override string[] ValidSortNames { get; }
            = new string[] {
                nameof(LogEntry.ID),
                nameof(LogEntry.Level),
                nameof(LogEntry.ScopeID),
                nameof(LogEntry.HappenedAt),
                nameof(LogEntry.Component),
                nameof(LogEntry.Application),
            };
    }
}
