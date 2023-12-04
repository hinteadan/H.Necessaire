using H.Necessaire.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Resources.Concrete
{
    internal class AuditFileSystemStorageResource : JsonCachedFileSystemStorageServiceBase<Guid, ImAnAuditEntry, AuditSearchFilter>, ImAnAuditingService
    {
        AuditMetadataFileSystemStorageResource auditMetadataStorageResource = null;
        ImAVersionProvider versionProvider = null;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            auditMetadataStorageResource = dependencyProvider.Get<AuditMetadataFileSystemStorageResource>();
            versionProvider = dependencyProvider.Get<ImAVersionProvider>();
        }

        public async Task Append<T>(ImAnAuditEntry metadata, T objectSnapshot)
        {
            await auditMetadataStorageResource.Save(await metadata.ToMeta().AndAsync(async x => x.AppVersion = await versionProvider?.GetCurrentVersion()));
            FileInfo payloadFile = BuildPayloadFile(metadata.ID);
            payloadFile.Directory.Create();
            using (FileStream payloadFileStream = payloadFile.Create())
            {
                await objectSnapshot.ToJsonObject(isPrettyPrinted: true).WriteToStreamAsync(payloadFileStream);
            }
        }

        public override async Task<OperationResult<IDisposableEnumerable<ImAnAuditEntry>>> Stream(AuditSearchFilter filter)
        {
            OperationResult<IDisposableEnumerable<ImAnAuditEntry>> metadataResult = await base.Stream(filter);
            if (!metadataResult.IsSuccessful || !(metadataResult.Payload?.Any() ?? false))
                return metadataResult;

            return
                OperationResult
                .Win()
                .WithPayload(
                    metadataResult.Payload.Select(x => BuildAuditEntry(x)).ToDisposableEnumerable()
                )
                ;
        }

        public async Task<ImAnAuditEntry[]> Browse(AuditSearchFilter filter)
        {
            return
                (await Stream(filter))?.ThrowOnFailOrReturn()?.ToArray();
        }

        protected override IEnumerable<ImAnAuditEntry> ApplyFilter(IEnumerable<ImAnAuditEntry> stream, AuditSearchFilter filter)
        {
            IEnumerable<ImAnAuditEntry> result = stream;

            if (filter?.IDs?.Any() ?? false)
            {
                result = result.Where(x => x.ID.In(filter.IDs));
            }

            if (filter?.AuditedObjectTypes?.Any() ?? false)
            {
                result = result.Where(x => x.AuditedObjectType.In(filter.AuditedObjectTypes));
            }

            if (filter?.AuditedObjectIDs?.Any() ?? false)
            {
                result = result.Where(x => x.AuditedObjectID.In(filter.AuditedObjectIDs));
            }

            if (filter?.ActionTypes?.Any() ?? false)
            {
                result = result.Where(x => x.ActionType.In(filter.ActionTypes));
            }

            if (filter?.DoneBy != null)
            {
                result = result.Where(x =>
                    ((filter.DoneBy?.IDs?.Any() ?? false) ? (x.DoneBy?.ID.In(filter.DoneBy.IDs) ?? false) : true)
                    || ((filter.DoneBy?.IDTags?.Any() ?? false) ? (x.DoneBy?.IDTag?.In(filter.DoneBy.IDTags) ?? false) : true)
                    || ((filter.DoneBy?.DisplayNames?.Any() ?? false) ? (x.DoneBy?.DisplayName?.In(filter.DoneBy.DisplayNames) ?? false) : true)
                );
            }

            if (filter?.FromInclusive != null)
            {
                result = result.Where(x => x.HappenedAt >= filter.FromInclusive.Value);
            }

            if (filter?.ToInclusive != null)
            {
                result = result.Where(x => x.HappenedAt <= filter.ToInclusive.Value);
            }

            result = result.ApplySortAndPageFilterIfAny(filter);

            return result;
        }

        private ImAnAuditEntry BuildAuditEntry(ImAnAuditEntry x)
        {
            return
                new JsonFileAuditEntry(x, BuildPayloadFile(x.ID));
        }

        private FileInfo BuildPayloadFile(Guid auditEntryID)
        {
            return new FileInfo(Path.Combine(entityStorageFolder.FullName, "ObjectSnapshots", $"{auditEntryID}.json"));
        }
    }

    internal class AuditMetadataFileSystemStorageResource : JsonCachedFileSystemStorageServiceBase<Guid, AuditMetadataEntry, AuditSearchFilter>
    {
        protected override IEnumerable<AuditMetadataEntry> ApplyFilter(IEnumerable<AuditMetadataEntry> stream, AuditSearchFilter filter)
        {
            IEnumerable<AuditMetadataEntry> result = stream;

            if (filter?.IDs?.Any() ?? false)
            {
                result = result.Where(x => x.ID.In(filter.IDs));
            }

            if (filter?.AuditedObjectTypes?.Any() ?? false)
            {
                result = result.Where(x => x.AuditedObjectType.In(filter.AuditedObjectTypes));
            }

            if (filter?.AuditedObjectIDs?.Any() ?? false)
            {
                result = result.Where(x => x.AuditedObjectID.In(filter.AuditedObjectIDs));
            }

            if (filter?.ActionTypes?.Any() ?? false)
            {
                result = result.Where(x => x.ActionType.In(filter.ActionTypes));
            }

            if (filter?.DoneBy != null)
            {
                result = result.Where(x =>
                    ((filter.DoneBy?.IDs?.Any() ?? false) ? (x.DoneBy?.ID.In(filter.DoneBy.IDs) ?? false) : true)
                    || ((filter.DoneBy?.IDTags?.Any() ?? false) ? (x.DoneBy?.IDTag?.In(filter.DoneBy.IDTags) ?? false) : true)
                    || ((filter.DoneBy?.DisplayNames?.Any() ?? false) ? (x.DoneBy?.DisplayName?.In(filter.DoneBy.DisplayNames) ?? false) : true)
                );
            }

            if (filter?.FromInclusive != null)
            {
                result = result.Where(x => x.HappenedAt >= filter.FromInclusive.Value);
            }

            if (filter?.ToInclusive != null)
            {
                result = result.Where(x => x.HappenedAt <= filter.ToInclusive.Value);
            }

            result = result.ApplySortAndPageFilterIfAny(filter);

            return result;
        }
    }
}
