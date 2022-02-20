using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;
using System;
using System.Linq;

namespace H.Necessaire.Runtime.RavenDB.Core.Resources
{
    internal class QdActionResultRavenDbStorageResource : RavenDbStorageServiceBase<Guid, QdActionResult, QdActionResultFilter, QdActionResultRavenDbStorageResource.QdActionResultFilterIndex>
    {
        protected override IAsyncDocumentQuery<QdActionResult> ApplyFilter(IAsyncDocumentQuery<QdActionResult> result, QdActionResultFilter filter)
        {
            if (filter?.IDs?.Any() ?? false)
            {
                result = result.WhereIn(nameof(QdActionResult.ID), filter.IDs.ToStringArray());
            }

            if (filter?.QdActionIDs?.Any() ?? false)
            {
                result = result.WhereIn(nameof(QdActionResult.QdActionID), filter.QdActionIDs.ToStringArray());
            }

            if (filter?.FromInclusive != null)
            {
                result = result.WhereGreaterThanOrEqual(nameof(QdActionResult.HappenedAt), filter.FromInclusive.Value);
            }

            if (filter?.ToInclusive != null)
            {
                result = result.WhereLessThanOrEqual(nameof(QdActionResult.HappenedAt), filter.ToInclusive.Value);
            }

            return result;
        }

        public class QdActionResultFilterIndex : AbstractIndexCreationTask<QdActionResult>
        {
            public QdActionResultFilterIndex()
            {
                Map = docs => docs.Select(doc =>
                    new
                    {
                        doc.ID,
                        doc.QdActionID,
                        doc.HappenedAt,
                        doc.IsSuccessful,
                    }
                );
            }
        }
    }
}
