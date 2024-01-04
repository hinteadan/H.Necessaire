using H.Necessaire.Runtime.Google.FirestoreDB.Core.Resources.Abstract;
using H.Necessaire.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Core.Resources
{
    internal class AuditGoogleFirestoreDbStorageResource : ImADependency, ImAnAuditingService
    {
        #region Construct
        AuditMetadataGoogleFirestoreDbStorageResource metadataStorage = null;
        AuditPayloadGoogleFirestoreDbStorageResource payloadStorage = null;
        ImAVersionProvider versionProvider = null;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            metadataStorage = dependencyProvider.Get<AuditMetadataGoogleFirestoreDbStorageResource>();
            payloadStorage = dependencyProvider.Get<AuditPayloadGoogleFirestoreDbStorageResource>();
            versionProvider = dependencyProvider.Get<ImAVersionProvider>();
        }
        #endregion

        public async Task Append<T>(ImAnAuditEntry metadata, T objectSnapshot)
        {
            await metadataStorage.Save(await metadata.ToMeta().AndAsync(async x => x.AppVersion = await versionProvider?.GetCurrentVersion()));
            await payloadStorage.Save(new AuditPayloadGoogleFirestoreDbStorageResource.AuditPayloadEntry
            {
                ID = $"PayloadFor-{metadata.ID}",
                PayloadAsJson = objectSnapshot.ToJsonObject(),
            });
        }

        public async Task<ImAnAuditEntry[]> Browse(AuditSearchFilter filter)
        {
            AuditMetadataEntry[] metadata = (await metadataStorage.LoadPage(filter))?.Payload?.Content;

            if (!metadata?.Any() ?? true)
                return new ImAnAuditEntry[0];

            return
                metadata
                .Select(x => new JsonAuditEntry(x, LoadAuditJsonPayload) as ImAnAuditEntry)
                .ToArray();
        }

        private async Task<Stream> LoadAuditJsonPayload(ImAnAuditEntry auditMetadata)
        {
            AuditPayloadGoogleFirestoreDbStorageResource.AuditPayloadEntry auditPayloadEntry = (await payloadStorage.LoadByID($"PayloadFor-{auditMetadata.ID}")).ThrowOnFailOrReturn();
            return auditPayloadEntry?.PayloadAsJson?.ToStream();
        }
    }

    internal class AuditMetadataGoogleFirestoreDbStorageResource
        : GoogleFirestoreDbStorageResourceBase<Guid, AuditMetadataEntry, AuditSearchFilter>
    {
        protected override IDictionary<string, Note> FilterToStoreMapping
            => new Dictionary<string, Note>() {
                { nameof(AuditSearchFilter.FromInclusive), nameof(AuditMetadataEntry.HappenedAt).NoteAs(">=") },
                { nameof(AuditSearchFilter.ToInclusive), nameof(AuditMetadataEntry.HappenedAt).NoteAs("<=") },
            };
    }

    internal class AuditPayloadGoogleFirestoreDbStorageResource
        : GoogleFirestoreDbStorageResourceBase<string, AuditPayloadGoogleFirestoreDbStorageResource.AuditPayloadEntry, IDFilter<string>>
    {
        public class AuditPayloadEntry : IStringIdentity
        {
            public string ID { get; set; }
            public string PayloadAsJson { get; set; }
        }
    }
}
