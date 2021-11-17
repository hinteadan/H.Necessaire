using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;
using System.Linq;

namespace H.Necessaire.Runtime.RavenDB
{
    internal class SyncRequestRavenDbStorageResource : RavenDbStorageServiceBase<string, SyncRequest, SyncRequestFilter, SyncRequestRavenDbStorageResource.SyncRequestFilterIndex>
    {
        protected override string DatabaseName { get; } = "H.Necessaire.Core";

        protected override IAsyncDocumentQuery<SyncRequest> ApplyFilter(IAsyncDocumentQuery<SyncRequest> result, SyncRequestFilter filter)
        {
            if (filter?.IDs?.Any() ?? false)
            {
                result = result.WhereIn(nameof(SyncRequest.ID), filter.IDs);
            }

            if (filter?.PayloadIdentifiers?.Any() ?? false)
            {
                result = result.WhereIn(nameof(SyncRequest.PayloadIdentifier), filter.PayloadIdentifiers);
            }

            if (filter?.PayloadTypes?.Any() ?? false)
            {
                result = result.WhereIn(nameof(SyncRequest.PayloadType), filter.PayloadTypes);
            }

            if (filter?.SyncStatuses?.Any() ?? false)
            {
                result = result.WhereIn(nameof(SyncRequest.SyncStatus), filter.SyncStatuses.Select(x => x.ToString()).ToArray());
            }

            if (filter?.FromInclusive != null)
            {
                result = result.WhereGreaterThanOrEqual(nameof(SyncRequest.HappenedAt), filter.FromInclusive.Value);
            }

            if (filter?.ToInclusive != null)
            {
                result = result.WhereLessThanOrEqual(nameof(SyncRequest.HappenedAt), filter.ToInclusive.Value);
            }

            return result;
        }

        public class SyncRequestFilterIndex : AbstractIndexCreationTask<SyncRequest>
        {
            public SyncRequestFilterIndex()
            {
                Map = docs => docs.Select(doc =>
                new
                {
                    doc.ID,
                    doc.PayloadIdentifier,
                    doc.PayloadType,
                    doc.HappenedAt,
                    doc.SyncStatus,
                }
            );
            }
        }
    }
}
