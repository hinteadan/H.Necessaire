using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;
using System.Linq;

namespace H.Necessaire.Runtime.RavenDB
{
    internal class ExiledSyncRequestRavenDbStorageResource : RavenDbStorageServiceBase<string, ExiledSyncRequest, SyncRequestFilter, ExiledSyncRequestRavenDbStorageResource.ExiledSyncRequestFilterIndex>
    {
        protected override IAsyncDocumentQuery<ExiledSyncRequest> ApplyFilter(IAsyncDocumentQuery<ExiledSyncRequest> result, SyncRequestFilter filter)
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

        protected override IDocumentQuery<ExiledSyncRequest> ApplyFilterSync(IDocumentQuery<ExiledSyncRequest> result, SyncRequestFilter filter)
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

        public class ExiledSyncRequestFilterIndex : AbstractIndexCreationTask<ExiledSyncRequest>
        {
            public ExiledSyncRequestFilterIndex()
            {
                Map = docs => docs.Select(doc =>
                    new
                    {
                        doc.ID,
                        doc.PayloadIdentifier,
                        doc.PayloadType,
                        doc.HappenedAt,
                        doc.SyncRequest.SyncStatus,
                        doc.SyncRequestProcessingResult.IsSuccessful,
                    }
                );
            }
        }
    }
}
