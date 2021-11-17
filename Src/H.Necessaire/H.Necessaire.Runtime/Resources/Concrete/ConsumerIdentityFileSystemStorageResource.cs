using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire.Runtime.Resources.Concrete
{
    internal class ConsumerIdentityFileSystemStorageResource : JsonCachedFileSystemStorageServiceBase<Guid, ConsumerIdentity, IDFilter<Guid>>
    {
        protected override IEnumerable<ConsumerIdentity> ApplyFilter(IEnumerable<ConsumerIdentity> stream, IDFilter<Guid> filter)
        {
            IEnumerable<ConsumerIdentity> result = stream;

            if (filter?.IDs?.Any() ?? false)
            {
                result = result.Where(x => x.ID.In(filter.IDs));
            }

            result = result.ApplySortAndPageFilterIfAny(filter);

            return result;
        }
    }
}
