using System;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public abstract class QdActionProcessorBase : ImAQdActionProcessor, ImADependency
    {
        #region Construct
        int maxProcessingAttempts = 3;
        string processorName;
        ImALogger logger;
        public virtual void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            processorName = this.GetType().TypeName();
            logger = dependencyProvider.GetLogger(this.GetType());
            maxProcessingAttempts = dependencyProvider.GetRuntimeConfig()?.Get("QdActions")?.Get("MaxProcessingAttempts")?.ToString()?.ParseToIntOrFallbackTo(3) ?? 3;
        }
        #endregion

        protected abstract string[] SupportedQdActionTypes { get; }

        public virtual Task<bool> IsEligibleFor(QdAction action)
        {
            return action.Type.In(SupportedQdActionTypes).AsTask();
        }

        public virtual async Task<QdActionResult> Process(QdAction action)
        {
            QdActionResult result = OperationResult.Fail("Not yet processed").WithPayload(action).ToQdActionResult();

            await
                new Func<Task>(async () =>
                {
                    if (action.Status.In(QdActionStatus.Succeeded))
                    {
                        result = OperationResult.Win("Successfully processed already").WithPayload(action).ToQdActionResult();
                        return;
                    }

                    if (action.Status.In(QdActionStatus.Failed) && action.RunCount >= maxProcessingAttempts)
                    {
                        result = OperationResult.Fail($"Will skip because the processing failed too many times ({action.RunCount}/{maxProcessingAttempts})").WithPayload(action).ToQdActionResult();
                        return;
                    }

                    await logger.LogTrace($"Running {processorName} for {action}");
                    using (new TimeMeasurement(async x => await logger.LogTrace($"DONE Running {processorName} for {action} in {x}")))
                    {
                        OperationResult processingResult = await ProcessQdAction(action);
                        result = processingResult.WithPayload(action).ToQdActionResult();
                    }
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        await logger.LogError(ex);
                        result = OperationResult.Fail(ex).WithPayload(action).ToQdActionResult();
                    }
                );

            return result;
        }

        protected abstract Task<OperationResult> ProcessQdAction(QdAction action);
    }
}
