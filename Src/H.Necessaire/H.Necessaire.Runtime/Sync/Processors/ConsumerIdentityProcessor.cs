﻿using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Sync.Processors
{
    internal class ConsumerIdentityProcessor : SyncRequestProcessorBase<ConsumerIdentity>, ImADependency
    {
        #region Construct
        static readonly IDentity processorIdentity = new InternalIdentity
        {
            ID = Guid.Parse("93CDC1B3-2FFA-45DE-A769-154B482E29A0"),
            IDTag = nameof(ConsumerIdentityProcessor),
            DisplayName = "Consumer Identity Processor",
        };

        ImAStorageService<Guid, ConsumerIdentity> consumerIdentityStorageService = null;
        ImAnAuditingService auditingService = null;
        ImAnActionQer actionQer = null;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            consumerIdentityStorageService = dependencyProvider.Get<ImAStorageService<Guid, ConsumerIdentity>>();
            auditingService = dependencyProvider.Get<ImAnAuditingService>();
            actionQer = dependencyProvider.Get<ImAnActionQer>();
        }
        #endregion

        protected override async Task<OperationResult> ProcessPayload(ConsumerIdentity payload, SyncRequest syncRequest)
        {
            ConsumerIdentity consumerIdentity = payload.And(x =>
            {
                x.ID = syncRequest.OperationContext?.Consumer?.ID ?? x.ID;
                x.Notes = x.Notes.AddOrReplace(syncRequest.OperationContext?.Consumer?.Notes);
            });

            bool consumerExists = (await consumerIdentityStorageService.LoadByID(consumerIdentity.ID))?.Payload != null;

            OperationResult result = await consumerIdentityStorageService.Save(consumerIdentity);
            if (!result.IsSuccessful)
                return result;

            await auditingService.Append(consumerIdentity.ToAuditMeta<ConsumerIdentity, Guid>(consumerExists ? AuditActionType.Modify : AuditActionType.Create, processorIdentity), consumerIdentity);

            if (!string.IsNullOrWhiteSpace(payload.IpAddress))
                await actionQer.Queue(QdAction.New(WellKnown.QdActionType.ProcessIpAddress, payload.IpAddress));

            return result;
        }
    }
}
