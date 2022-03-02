using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire.Runtime.Resources.Concrete
{
    internal class RuntimeTraceFileSystemStorageResource : JsonCachedFileSystemStorageServiceBase<Guid, RuntimeTrace, IDFilter<Guid>>
    {
        protected override IEnumerable<RuntimeTrace> ApplyFilter(IEnumerable<RuntimeTrace> stream, IDFilter<Guid> filter)
        {
            IEnumerable<RuntimeTrace> result = stream;

            if (filter?.IDs?.Any() ?? false)
            {
                result = result.Where(x => x.ID.In(filter.IDs));
            }

            result = result.ApplySortAndPageFilterIfAny(filter);

            return result;
        }
    }
}
