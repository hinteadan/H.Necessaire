using H.Necessaire.Serialization;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Sync.Processors
{
    internal class ConsumerIdentityProcessor : ImASyncRequestProcessor, ImADependency
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
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            consumerIdentityStorageService = dependencyProvider.Get<ImAStorageService<Guid, ConsumerIdentity>>();
            auditingService = dependencyProvider.Get<ImAnAuditingService>();
        }
        #endregion

        public Task<bool> IsEligibleFor(SyncRequest syncRequest)
        {
            return syncRequest.PayloadType.In(typeof(ConsumerIdentity).TypeName()).AsTask();
        }

        public async Task<OperationResult> Process(SyncRequest syncRequest)
        {
            OperationResult result = OperationResult.Fail();

            await
                new Func<Task>(async () =>
                {
                    OperationResult<ConsumerIdentity> parseOperation = syncRequest.Payload.TryJsonToObject<ConsumerIdentity>();

                    if (!parseOperation.IsSuccessful)
                    {
                        result = parseOperation;
                        return;
                    }
                    if (parseOperation.Payload == null)
                    {
                        result = OperationResult.Fail("The sync request payload is empty");
                        return;
                    }

                    ConsumerIdentity consumerIdentity = parseOperation.Payload.And(x =>
                    {
                        x.ID = syncRequest.OperationContext?.Consumer?.ID ?? x.ID;
                        x.Notes = x.Notes.AddOrReplace(syncRequest.OperationContext?.Consumer?.Notes);
                    });

                    bool consumerExists = (await consumerIdentityStorageService.LoadByID(consumerIdentity.ID))?.Payload != null;

                    result = await consumerIdentityStorageService.Save(consumerIdentity);

                    await auditingService.Append(consumerIdentity.ToAuditMeta<ConsumerIdentity, Guid>(consumerExists ? AuditActionType.Modify : AuditActionType.Create, processorIdentity), consumerIdentity);
                })
                .TryOrFailWithGrace(onFail: ex => result = OperationResult.Fail(ex));

            return result;
        }
    }
}
