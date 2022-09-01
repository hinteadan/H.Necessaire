using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;
using System;
using System.Linq;

namespace H.Necessaire.Runtime.RavenDB.Core.Resources
{
    internal class ConsumerIdentityRavenDbStorageResource : RavenDbStorageServiceBase<Guid, ConsumerIdentity, IDFilter<Guid>, ConsumerIdentityRavenDbStorageResource.ConsumerIdentityFilterIndex>
    {
        protected override IAsyncDocumentQuery<ConsumerIdentity> ApplyFilter(IAsyncDocumentQuery<ConsumerIdentity> result, IDFilter<Guid> filter)
        {
            if (filter?.IDs?.Any() ?? false)
            {
                result = result.WhereIn(nameof(ConsumerIdentity.ID), filter.IDs.ToStringArray());
            }

            return result;
        }

        protected override IDocumentQuery<ConsumerIdentity> ApplyFilterSync(IDocumentQuery<ConsumerIdentity> result, IDFilter<Guid> filter)
        {
            if (filter?.IDs?.Any() ?? false)
            {
                result = result.WhereIn(nameof(ConsumerIdentity.ID), filter.IDs.ToStringArray());
            }

            return result;
        }

        public class ConsumerIdentityFilterIndex : AbstractIndexCreationTask<ConsumerIdentity>
        {
            public ConsumerIdentityFilterIndex()
            {
                Map = docs => docs.Select(doc =>
                    new
                    {
                        doc.ID,
                        doc.IDTag,
                        doc.DisplayName,
                        doc.AsOf,
                    }
                );
            }
        }
    }
}
