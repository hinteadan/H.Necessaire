using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire.Runtime.Resources.Concrete
{
    internal class SyncRequestFileSystemStorageResource : JsonCachedFileSystemStorageServiceBase<string, SyncRequest, SyncRequestFilter>
    {
        protected override IEnumerable<SyncRequest> ApplyFilter(IEnumerable<SyncRequest> stream, SyncRequestFilter filter)
        {
            IEnumerable<SyncRequest> result = stream;

            if (filter?.IDs?.Any() ?? false)
            {
                result = result.Where(x => x.ID.In(filter.IDs));
            }

            if (filter?.PayloadIdentifiers?.Any() ?? false)
            {
                result = result.Where(x => x.PayloadIdentifier.In(filter.PayloadIdentifiers));
            }

            if (filter?.PayloadTypes?.Any() ?? false)
            {
                result = result.Where(x => x.PayloadType.In(filter.PayloadTypes));
            }

            if (filter?.SyncStatuses?.Any() ?? false)
            {
                result = result.Where(x => x.SyncStatus.In(filter.SyncStatuses));
            }

            if (filter?.FromInclusive != null)
            {
                result = result.Where(x => x.HappenedAt >= filter.FromInclusive.Value);
            }

            if (filter?.ToInclusive != null)
            {
                result = result.Where(x => x.HappenedAt <= filter.ToInclusive.Value);
            }

            result = result.ApplySortAndPageFilterIfAny(filter);

            return result;
        }
    }
}
