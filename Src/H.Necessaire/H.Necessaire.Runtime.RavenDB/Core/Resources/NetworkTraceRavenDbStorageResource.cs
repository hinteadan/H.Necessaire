using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;
using System;
using System.Linq;

namespace H.Necessaire.Runtime.RavenDB.Core.Resources
{
    internal class NetworkTraceRavenDbStorageResource : RavenDbStorageServiceBase<Guid, NetworkTrace, IDFilter<Guid>, NetworkTraceRavenDbStorageResource.NetworkTraceFilterIndex>
    {
        protected override IAsyncDocumentQuery<NetworkTrace> ApplyFilter(IAsyncDocumentQuery<NetworkTrace> result, IDFilter<Guid> filter)
        {
            if (filter?.IDs?.Any() ?? false)
            {
                result = result.WhereIn(nameof(NetworkTrace.ID), filter.IDs.ToStringArray());
            }

            return result;
        }

        protected override IDocumentQuery<NetworkTrace> ApplyFilterSync(IDocumentQuery<NetworkTrace> result, IDFilter<Guid> filter)
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
