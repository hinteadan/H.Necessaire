using H.Necessaire.Runtime.Azure.CosmosDB.Core.Resources.Abstract;
using H.Necessaire.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Azure.CosmosDB.Core.Resources
{
    internal class AuditAzureCosmosDbStorageResource : ImADependency, ImAnAuditingService
    {
        #region Construct
        AuditMetadataAzureCosmosDbStorageResource metadataStorage = null;
        AuditPayloadAzureCosmosDbStorageResource payloadStorage = null;
        ImAVersionProvider versionProvider = null;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            metadataStorage = dependencyProvider.Get<AuditMetadataAzureCosmosDbStorageResource>();
            payloadStorage = dependencyProvider.Get<AuditPayloadAzureCosmosDbStorageResource>();
            versionProvider = dependencyProvider.Get<ImAVersionProvider>();
        }
        #endregion

        public async Task Append<T>(ImAnAuditEntry metadata, T objectSnapshot)
        {
            await metadataStorage.Save(await metadata.ToMeta().AndAsync(async x => x.AppVersion = await versionProvider?.GetCurrentVersion()));
            await payloadStorage.Save(new AuditPayloadAzureCosmosDbStorageResource.AuditPayloadEntry
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
            AuditPayloadAzureCosmosDbStorageResource.AuditPayloadEntry auditPayloadEntry = (await payloadStorage.LoadByID($"PayloadFor-{auditMetadata.ID}")).ThrowOnFailOrReturn();
            return auditPayloadEntry?.PayloadAsJson?.ToStream();
        }
    }

    internal class AuditMetadataAzureCosmosDbStorageResource
        : AzureCosmosDbStorageResourceBase<Guid, AuditMetadataEntry, AuditSearchFilter>
    {
        protected override IDictionary<string, string> FilterToStoreMapping
            => new Dictionary<string, string>() {
                { nameof(AuditSearchFilter.FromInclusive), nameof(AuditMetadataEntry.HappenedAt) },
                { nameof(AuditSearchFilter.ToInclusive), nameof(AuditMetadataEntry.HappenedAt) },
            };
    }

    internal class AuditPayloadAzureCosmosDbStorageResource
        : AzureCosmosDbStorageResourceBase<string, AuditPayloadAzureCosmosDbStorageResource.AuditPayloadEntry, IDFilter<string>>
    {
        public class AuditPayloadEntry : IStringIdentity
        {
            public string ID { get; set; }
            public string PayloadAsJson { get; set; }
        }
    }
}
