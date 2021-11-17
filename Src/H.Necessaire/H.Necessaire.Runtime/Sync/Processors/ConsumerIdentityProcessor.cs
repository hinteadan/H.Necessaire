using H.Necessaire.Serialization;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Sync.Processors
{
    internal class ConsumerIdentityProcessor : ImASyncRequestProcessor, ImADependency
    {
        #region Construct
        ImAStorageService<Guid, ConsumerIdentity> consumerIdentityStorageService = null;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            consumerIdentityStorageService = dependencyProvider.Get<ImAStorageService<Guid, ConsumerIdentity>>();
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

                    result = await consumerIdentityStorageService.Save(consumerIdentity);

                    //TODO: Add Auditing here
                })
                .TryOrFailWithGrace(onFail: ex => result = OperationResult.Fail(ex));

            return result;
        }
    }
}
