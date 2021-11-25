using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire.Runtime.Resources.Concrete
{
    internal class QdActionResultFileSystemStorageResource : JsonCachedFileSystemStorageServiceBase<Guid, QdActionResult, QdActionResultFilter>
    {
        protected override IEnumerable<QdActionResult> ApplyFilter(IEnumerable<QdActionResult> stream, QdActionResultFilter filter)
        {
            IEnumerable<QdActionResult> result = stream;

            if (filter?.IDs?.Any() ?? false)
            {
                result = result.Where(x => x.ID.In(filter.IDs));
            }

            if (filter?.QdActionIDs?.Any() ?? false)
            {
                result = result.Where(x => x.QdActionID.In(filter.QdActionIDs));
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
