using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;
using System;
using System.Linq;

namespace H.Necessaire.Runtime.RavenDB.Core.Resources
{
    internal class RuntimeTraceRavenDbStorageResource : RavenDbStorageServiceBase<Guid, RuntimeTrace, IDFilter<Guid>, RuntimeTraceRavenDbStorageResource.RuntimeTraceFilterIndex>
    {
        protected override IAsyncDocumentQuery<RuntimeTrace> ApplyFilter(IAsyncDocumentQuery<RuntimeTrace> result, IDFilter<Guid> filter)
        {
            if (filter?.IDs?.Any() ?? false)
            {
                result = result.WhereIn(nameof(RuntimeTrace.ID), filter.IDs.ToStringArray());
            }

            return result;
        }

        public class RuntimeTraceFilterIndex : AbstractIndexCreationTask<RuntimeTrace>
        {
            public RuntimeTraceFilterIndex()
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
