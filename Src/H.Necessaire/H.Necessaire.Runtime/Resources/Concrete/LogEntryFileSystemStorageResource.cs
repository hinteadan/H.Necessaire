using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire.Runtime.Resources.Concrete
{
    internal class LogEntryFileSystemStorageResource : JsonCachedFileSystemStorageServiceBase<Guid, LogEntry, LogFilter>
    {
        protected override IEnumerable<LogEntry> ApplyFilter(IEnumerable<LogEntry> stream, LogFilter filter)
        {
            IEnumerable<LogEntry> result = stream;

            if (filter?.IDs?.Any() ?? false)
                result = result.Where(x => x.ID.In(filter.IDs));

            if (filter?.Levels?.Any() ?? false)
                result = result.Where(x => x.Level.In(filter.Levels));

            if (filter?.ScopeIDs?.Any() ?? false)
                result = result.Where(x => x.ScopeID.In(filter.ScopeIDs));

            if (filter?.Methods?.Any() ?? false)
                result = result.Where(x => x.Method.In(filter.Methods, (a, b) => string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase)));

            if (filter?.Components?.Any() ?? false)
                result = result.Where(x => x.Component.In(filter.Components, (a, b) => string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase)));

            if (filter?.Applications?.Any() ?? false)
                result = result.Where(x => x.Application.In(filter.Applications, (a, b) => string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase)));

            if (filter?.FromInclusive != null)
                result = result.Where(x => x.HappenedAt >= filter.FromInclusive.Value);

            if (filter?.ToInclusive != null)
                result = result.Where(x => x.HappenedAt <= filter.ToInclusive.Value);

            return result.ApplySortAndPageFilterIfAny(filter);
        }
    }
}
