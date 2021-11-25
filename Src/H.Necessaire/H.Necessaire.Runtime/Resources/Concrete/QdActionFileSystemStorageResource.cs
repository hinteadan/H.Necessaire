using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire.Runtime.Resources.Concrete
{
    internal class QdActionFileSystemStorageResource : JsonCachedFileSystemStorageServiceBase<Guid, QdAction, QdActionFilter>
    {
        protected override IEnumerable<QdAction> ApplyFilter(IEnumerable<QdAction> stream, QdActionFilter filter)
        {
            IEnumerable<QdAction> result = stream;

            if (filter?.IDs?.Any() ?? false)
            {
                result = result.Where(x => x.ID.In(filter.IDs));
            }

            if (filter?.Types?.Any() ?? false)
            {
                result = result.Where(x => x.Type.In(filter.Types));
            }

            if (filter?.Statuses?.Any() ?? false)
            {
                result = result.Where(x => x.Status.In(filter.Statuses));
            }

            if (filter?.MinRunCount != null)
            {
                result = result.Where(x => x.RunCount >= filter.MinRunCount.Value);
            }

            if (filter?.MaxRunCount != null)
            {
                result = result.Where(x => x.RunCount <= filter.MaxRunCount.Value);
            }

            if (filter?.FromInclusive != null)
            {
                result = result.Where(x => x.QdAt >= filter.FromInclusive.Value);
            }

            if (filter?.ToInclusive != null)
            {
                result = result.Where(x => x.QdAt <= filter.ToInclusive.Value);
            }

            result = result.ApplySortAndPageFilterIfAny(filter);

            return result;
        }
    }
}
