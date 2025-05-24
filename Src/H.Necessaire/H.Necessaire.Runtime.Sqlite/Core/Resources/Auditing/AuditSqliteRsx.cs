using H.Necessaire.Serialization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Sqlite.Core.Resources.Auditing
{
    internal class AuditSqliteRsx : ImADependency, ImAnAuditingService
    {
        #region Construct
        AuditMetadataSqliteRsx metadataStorage = null;
        AuditPayloadSqliteRsx payloadStorage = null;
        ImAVersionProvider versionProvider = null;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            metadataStorage = dependencyProvider.Get<AuditMetadataSqliteRsx>();
            payloadStorage = dependencyProvider.Get<AuditPayloadSqliteRsx>();
            versionProvider = dependencyProvider.Get<ImAVersionProvider>();
        }
        #endregion

        public async Task Append<T>(ImAnAuditEntry metadata, T objectSnapshot)
        {
            (await metadataStorage.Save(await metadata.ToMeta().AndAsync(async x => x.AppVersion = await versionProvider?.GetCurrentVersion()))).ThrowOnFail();
            (await payloadStorage.Save(new AuditPayloadSqliteRsx.AuditPayloadSqlEntry
            {
                ID = metadata.ID,
                PayloadAsJson = objectSnapshot.ToJsonObject(),
            })).ThrowOnFail();
        }

        public async Task<ImAnAuditEntry[]> Browse(AuditSearchFilter filter)
        {
            AuditMetadataEntry[] metadata = (await metadataStorage.LoadPage(filter)).ThrowOnFailOrReturn().Content;

            if (!metadata?.Any() ?? true)
                return new ImAnAuditEntry[0];

            return
                metadata
                .Select(x => new JsonAuditEntry(x, LoadAuditJsonPayload) as ImAnAuditEntry)
                .ToArray();
        }

        private async Task<Stream> LoadAuditJsonPayload(ImAnAuditEntry auditMetadata)
        {
            AuditPayloadSqliteRsx.AuditPayloadSqlEntry auditPayloadEntry = (await payloadStorage.LoadByID(auditMetadata.ID)).ThrowOnFailOrReturn();
            return auditPayloadEntry?.PayloadAsJson?.ToStream();
        }
    }
}
