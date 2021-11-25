using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire.Runtime.Resources.Concrete
{
    internal class NetworkTraceFileSystemStorageResource : JsonCachedFileSystemStorageServiceBase<Guid, NetworkTrace, IDFilter<Guid>>
    {
        protected override IEnumerable<NetworkTrace> ApplyFilter(IEnumerable<NetworkTrace> stream, IDFilter<Guid> filter)
        {
            IEnumerable<NetworkTrace> result = stream;

            if (filter?.IDs?.Any() ?? false)
            {
                result = result.Where(x => x.ID.In(filter.IDs));
            }

            result = result.ApplySortAndPageFilterIfAny(filter);

            return result;
        }
    }
}
