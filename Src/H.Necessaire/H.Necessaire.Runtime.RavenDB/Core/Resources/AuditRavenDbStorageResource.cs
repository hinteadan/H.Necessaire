using H.Necessaire.Serialization;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.RavenDB.Core.Resources
{
    internal class AuditRavenDbStorageResource : ImADependency, ImAnAuditingService
    {
        #region Construct
        AuditMetadataRavenDbStorageResource metadataStorage = null;
        AuditPayloadRavenDbStorageResource payloadStorage = null;
        ImAVersionProvider versionProvider = null;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            metadataStorage = dependencyProvider.Get<AuditMetadataRavenDbStorageResource>();
            payloadStorage = dependencyProvider.Get<AuditPayloadRavenDbStorageResource>();
            versionProvider = dependencyProvider.Get<ImAVersionProvider>();
        }
        #endregion

        public async Task Append<T>(ImAnAuditEntry metadata, T objectSnapshot)
        {
            await metadataStorage.Save(await metadata.ToMeta().AndAsync(async x => x.AppVersion = await versionProvider?.GetCurrentVersion()));
            await payloadStorage.Save(new AuditPayloadRavenDbStorageResource.AuditPayloadEntry
            {
                ID = $"PayloadFor-{metadata.ID}",
                PayloadAsJson = objectSnapshot.ToJsonObject(),
            });
        }

        public async Task<ImAnAuditEntry[]> Browse(AuditSearchFilter filter)
        {
            AuditMetadataEntry[] metadata = await metadataStorage.Search(filter);

            if (!metadata?.Any() ?? true)
                return new ImAnAuditEntry[0];

            return
                metadata
                .Select(x => new JsonAuditEntry(x, LoadAuditJsonPayload) as ImAnAuditEntry)
                .ToArray();
        }

        private async Task<Stream> LoadAuditJsonPayload(ImAnAuditEntry auditMetadata)
        {
            AuditPayloadRavenDbStorageResource.AuditPayloadEntry auditPayloadEntry = (await payloadStorage.LoadByID($"PayloadFor-{auditMetadata.ID}")).ThrowOnFailOrReturn();
            return auditPayloadEntry?.PayloadAsJson?.ToStream();
        }
    }

    internal class AuditPayloadRavenDbStorageResource : RavenDbStorageServiceBase<string, AuditPayloadRavenDbStorageResource.AuditPayloadEntry, IDFilter<string>, AuditPayloadRavenDbStorageResource.AuditPayloadFilterIndex>
    {
        protected override TDocQuery ApplyFilterGeneric<TDocQuery>(TDocQuery result, IDFilter<string> filter)
        {
            if (filter?.IDs?.Any() ?? false)
            {
                result = result.WhereIn(nameof(AuditMetadataEntry.ID), filter.IDs.ToStringArray());
            }

            return result;
        }

        public class AuditPayloadEntry : IStringIdentity
        {
            public string ID { get; set; }
            public string PayloadAsJson { get; set; }
        }

        public class AuditPayloadFilterIndex : AbstractIndexCreationTask<AuditPayloadEntry>
        {
            public AuditPayloadFilterIndex()
            {
                Map = docs => docs.Select(doc => new
                {
                    doc.ID,
                });
            }
        }
    }

    internal class AuditMetadataRavenDbStorageResource : RavenDbStorageServiceBase<Guid, AuditMetadataEntry, AuditSearchFilter, AuditMetadataRavenDbStorageResource.AuditSearchFilterIndex>
    {
        protected override TDocQuery ApplyFilterGeneric<TDocQuery>(TDocQuery result, AuditSearchFilter filter)
        {
            if (filter?.IDs?.Any() ?? false)
            {
                result = result.WhereIn(nameof(AuditMetadataEntry.ID), filter.IDs.ToStringArray());
            }

            if (filter?.AuditedObjectTypes?.Any() ?? false)
            {
                result = result.WhereIn(nameof(AuditMetadataEntry.AuditedObjectType), filter.AuditedObjectTypes);
            }

            if (filter?.AuditedObjectIDs?.Any() ?? false)
            {
                result = result.WhereIn(nameof(AuditMetadataEntry.AuditedObjectID), filter.AuditedObjectIDs);
            }

            if (filter?.DoneBy?.IDs?.Any() ?? false)
            {
                result = result.WhereIn(nameof(AuditMetadataEntry.DoneBy.ID), filter.DoneBy.IDs.ToStringArray());
            }

            if (filter?.DoneBy?.IDTags?.Any() ?? false)
            {
                result = result.WhereIn(nameof(AuditMetadataEntry.DoneBy.IDTag), filter.DoneBy.IDTags);
            }

            if (filter?.DoneBy?.DisplayNames?.Any() ?? false)
            {
                result = result.WhereIn(nameof(AuditMetadataEntry.DoneBy.DisplayName), filter.DoneBy.DisplayNames);
            }

            if (filter?.ActionTypes?.Any() ?? false)
            {
                result = result.WhereIn(nameof(AuditMetadataEntry.ActionType), filter.ActionTypes.ToStringArray());
            }

            if (filter?.FromInclusive != null)
            {
                result = result.WhereGreaterThanOrEqual(nameof(AuditMetadataEntry.HappenedAt), filter.FromInclusive.Value);
            }

            if (filter?.ToInclusive != null)
            {
                result = result.WhereLessThanOrEqual(nameof(AuditMetadataEntry.HappenedAt), filter.ToInclusive.Value);
            }

            return result;
        }

        public class AuditSearchFilterIndex : AbstractIndexCreationTask<AuditMetadataEntry>
        {
            public AuditSearchFilterIndex()
            {
                Map = docs => docs.Select(doc =>
                    new
                    {
                        doc.ID,
                        doc.AuditedObjectType,
                        doc.AuditedObjectID,
                        doc.HappenedAt,
                        DoneByID = doc.DoneBy.ID,
                        DoneByIDTag = doc.DoneBy.IDTag,
                        DoneByDisplayName = doc.DoneBy.DisplayName,
                        doc.ActionType,
                    }
                );
            }
        }
    }
}
