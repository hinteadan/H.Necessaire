using H.Necessaire.Serialization;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime
{
    public abstract class SyncRequestProcessorBase<TEntity> : ImASyncRequestProcessor, ImADependency
    {
        ImALogger logger = null;
        public virtual void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            logger = dependencyProvider.GetLogger<SyncRequestProcessorBase<TEntity>>();
        }

        public virtual Task<bool> IsEligibleFor(SyncRequest syncRequest)
        {
            return syncRequest.PayloadType.In(typeof(TEntity).TypeName()).AsTask();
        }

        public virtual async Task<OperationResult> Process(SyncRequest syncRequest)
        {
            OperationResult result = OperationResult.Fail();

            await
                new Func<Task>(async () =>
                {
                    if (syncRequest.SyncStatus == SyncStatus.DeletedAndNotSynced)
                    {
                        result = await ProcessPayload(string.IsNullOrWhiteSpace(syncRequest.Payload) ? default(TEntity) : syncRequest.Payload.JsonToObject<TEntity>(), syncRequest);
                        return;
                    }

                    OperationResult<TEntity> parseOperation = syncRequest.Payload.TryJsonToObject<TEntity>();

                    if (!parseOperation.IsSuccessful)
                    {
                        result = parseOperation;
                        return;
                    }
                    if (parseOperation.Payload == null)
                    {
                        result = OperationResult.Fail("The sync request payload is empty and the sync request is not for a delete operation");
                        return;
                    }

                    result = await ProcessPayload(parseOperation.Payload, syncRequest);
                })
                .TryOrFailWithGrace(onFail: async ex =>
                {
                    await logger?.LogError(ex);
                    result = OperationResult.Fail(ex);
                });

            return result;
        }

        protected abstract Task<OperationResult> ProcessPayload(TEntity payload, SyncRequest syncRequest);
    }
}
