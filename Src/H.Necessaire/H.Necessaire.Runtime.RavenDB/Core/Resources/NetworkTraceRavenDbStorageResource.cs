using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;
using System;
using System.Linq;

namespace H.Necessaire.Runtime.RavenDB.Core.Resources
{
    internal class NetworkTraceRavenDbStorageResource : RavenDbStorageServiceBase<Guid, NetworkTrace, IDFilter<Guid>, NetworkTraceRavenDbStorageResource.NetworkTraceFilterIndex>
    {
        protected override TDocQuery ApplyFilterGeneric<TDocQuery>(TDocQuery result, IDFilter<Guid> filter)
        {
            if (filter?.IDs?.Any() ?? false)
            {
                result = result.WhereIn(nameof(NetworkTrace.ID), filter.IDs.ToStringArray());
            }

            return result;
        }

        public class NetworkTraceFilterIndex : AbstractIndexCreationTask<NetworkTrace>
        {
            public NetworkTraceFilterIndex()
            {
                Map = docs => docs.Select(doc =>
                    new
                    {
                        doc.ID,
                    }
                );
            }
        }
    }
}
