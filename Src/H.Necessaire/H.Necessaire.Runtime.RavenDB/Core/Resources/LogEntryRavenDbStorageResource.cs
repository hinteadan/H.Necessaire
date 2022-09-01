using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;
using System;
using System.Linq;

namespace H.Necessaire.Runtime.RavenDB.Core.Resources
{
    internal class LogEntryRavenDbStorageResource : RavenDbStorageServiceBase<Guid, LogEntry, LogFilter, LogEntryRavenDbStorageResource.LogFilterIndex>
    {
        protected override IAsyncDocumentQuery<LogEntry> ApplyFilter(IAsyncDocumentQuery<LogEntry> result, LogFilter filter)
        {
            if (filter?.IDs?.Any() ?? false)
            {
                result = result.WhereIn(nameof(LogEntry.ID), filter.IDs.ToStringArray());
            }

            if (filter?.Levels?.Any() ?? false)
            {
                result = result.WhereIn(nameof(LogEntry.Level), filter.Levels.ToStringArray());
            }

            if (filter?.ScopeIDs?.Any() ?? false)
            {
                result = result.WhereIn(nameof(LogEntry.ScopeID), filter.ScopeIDs.ToStringArray());
            }

            if (filter?.Methods?.Any() ?? false)
            {
                result = result.WhereIn(nameof(LogEntry.Method), filter.Methods.ToStringArray());
            }

            if (filter?.Components?.Any() ?? false)
            {
                result = result.WhereIn(nameof(LogEntry.Component), filter.Components);
            }

            if (filter?.Applications?.Any() ?? false)
            {
                result = result.WhereIn(nameof(LogEntry.Application), filter.Applications);
            }

            if (filter?.FromInclusive != null)
            {
                result = result.WhereGreaterThanOrEqual(nameof(LogEntry.HappenedAt), filter.FromInclusive.Value);
            }

            if (filter?.ToInclusive != null)
            {
                result = result.WhereLessThanOrEqual(nameof(LogEntry.HappenedAt), filter.ToInclusive.Value);
            }

            return result;
        }

        protected override IDocumentQuery<LogEntry> ApplyFilterSync(IDocumentQuery<LogEntry> result, LogFilter filter)
        {
            if (filter?.IDs?.Any() ?? false)
            {
                result = result.WhereIn(nameof(LogEntry.ID), filter.IDs.ToStringArray());
            }

            if (filter?.Levels?.Any() ?? false)
            {
                result = result.WhereIn(nameof(LogEntry.Level), filter.Levels.ToStringArray());
            }

            if (filter?.ScopeIDs?.Any() ?? false)
            {
                result = result.WhereIn(nameof(LogEntry.ScopeID), filter.ScopeIDs.ToStringArray());
            }

            if (filter?.Methods?.Any() ?? false)
            {
                result = result.WhereIn(nameof(LogEntry.Method), filter.Methods.ToStringArray());
            }

            if (filter?.Components?.Any() ?? false)
            {
                result = result.WhereIn(nameof(LogEntry.Component), filter.Components);
            }

            if (filter?.Applications?.Any() ?? false)
            {
                result = result.WhereIn(nameof(LogEntry.Application), filter.Applications);
            }

            if (filter?.FromInclusive != null)
            {
                result = result.WhereGreaterThanOrEqual(nameof(LogEntry.HappenedAt), filter.FromInclusive.Value);
            }

            if (filter?.ToInclusive != null)
            {
                result = result.WhereLessThanOrEqual(nameof(LogEntry.HappenedAt), filter.ToInclusive.Value);
            }

            return result;
        }

        public class LogFilterIndex : AbstractIndexCreationTask<LogEntry>
        {
            public LogFilterIndex()
            {
                Map = docs => docs.Select(doc =>
                    new
                    {
                        doc.ID,
                        doc.Level,
                        doc.ScopeID,
                        doc.Method,
                        doc.Component,
                        doc.Application,
                        doc.HappenedAt,
                    }
                );
            }
        }
    }
}
