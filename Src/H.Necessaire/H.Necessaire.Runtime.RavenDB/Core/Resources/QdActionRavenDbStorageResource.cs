using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;
using System;
using System.Linq;

namespace H.Necessaire.Runtime.RavenDB.Core.Resources
{
    internal class QdActionRavenDbStorageResource : RavenDbStorageServiceBase<Guid, QdAction, QdActionFilter, QdActionRavenDbStorageResource.QdActionFilterIndex>
    {
        protected override string DatabaseName { get; } = "H.Necessaire.Core";

        protected override IAsyncDocumentQuery<QdAction> ApplyFilter(IAsyncDocumentQuery<QdAction> result, QdActionFilter filter)
        {
            if (filter?.IDs?.Any() ?? false)
            {
                result = result.WhereIn(nameof(QdAction.ID), filter.IDs.ToStringArray());
            }

            if (filter?.Types?.Any() ?? false)
            {
                result = result.WhereIn(nameof(QdAction.Type), filter.Types.ToStringArray());
            }

            if (filter?.Statuses?.Any() ?? false)
            {
                result = result.WhereIn(nameof(QdAction.Status), filter.Statuses.ToStringArray());
            }

            if (filter?.MinRunCount != null)
            {
                result = result.WhereGreaterThanOrEqual(nameof(QdAction.RunCount), filter.MinRunCount.Value);
            }

            if (filter?.MaxRunCount != null)
            {
                result = result.WhereLessThanOrEqual(nameof(QdAction.RunCount), filter.MaxRunCount.Value);
            }

            if (filter?.FromInclusive != null)
            {
                result = result.WhereGreaterThanOrEqual(nameof(QdAction.QdAt), filter.FromInclusive.Value);
            }

            if (filter?.ToInclusive != null)
            {
                result = result.WhereLessThanOrEqual(nameof(QdAction.QdAt), filter.ToInclusive.Value);
            }

            return result;
        }

        public class QdActionFilterIndex : AbstractIndexCreationTask<QdAction>
        {
            public QdActionFilterIndex()
            {
                Map = docs => docs.Select(doc =>
                    new
                    {
                        doc.ID,
                        doc.QdAt,
                        doc.Type,
                        doc.Status,
                        doc.RunCount,
                    }
                );
            }
        }
    }
}
