﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public class QdActionProcessingDaemon : ImADaemon, ImAQdActionQueueOnDemandRunner, ImADependency
    {
        #region Construct
        int maxProcessingAttempts = 3;
        int processingBatchSize = 10;
        TimeSpan processingInterval = TimeSpan.FromSeconds(15);
        ImALogger logger;
        ImAPeriodicAction processingTimer;
        ImAStorageService<Guid, QdAction> qdActionStorage;
        ImAStorageService<Guid, QdActionResult> qdActionResultStorage;
        ImAStorageBrowserService<QdAction, QdActionFilter> qdActionBrowser;
        ImAQdActionProcessor[] allKnownProcessors;
        public virtual void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            logger = dependencyProvider.GetLogger<QdActionProcessingDaemon>();
            processingTimer = dependencyProvider.Get<ImAPeriodicAction>();
            qdActionStorage = dependencyProvider.Get<ImAStorageService<Guid, QdAction>>();
            qdActionResultStorage = dependencyProvider.Get<ImAStorageService<Guid, QdActionResult>>();
            qdActionBrowser = dependencyProvider.Get<ImAStorageBrowserService<QdAction, QdActionFilter>>();
            int? configProcessingIntervalInSeconds = dependencyProvider.GetRuntimeConfig()?.Get("QdActions")?.Get("ProcessingIntervalInSeconds")?.ToString()?.ParseToIntOrFallbackTo(null);
            processingInterval = configProcessingIntervalInSeconds != null ? TimeSpan.FromSeconds(configProcessingIntervalInSeconds.Value) : processingInterval;
            maxProcessingAttempts = dependencyProvider.GetRuntimeConfig()?.Get("QdActions")?.Get("MaxProcessingAttempts")?.ToString()?.ParseToIntOrFallbackTo(3) ?? 3;
            processingBatchSize = dependencyProvider.GetRuntimeConfig()?.Get("QdActions")?.Get("ProcessingBatchSize")?.ToString()?.ParseToIntOrFallbackTo(10) ?? 10;
            allKnownProcessors = typeof(ImAQdActionProcessor).GetAllImplementations().Select(t => dependencyProvider.Get(t) as ImAQdActionProcessor).Where(x => x != null).ToArray();
        }
        #endregion

        public async Task Start(CancellationToken? cancellationToken = null)
        {
            OperationResult wireupValidationResult = await EnsureWireup();
            if (!wireupValidationResult.IsSuccessful)
            {
                await logger.LogWarn($"Cannot start QdAction Processing Daemon because the wireup is invalid.", wireupValidationResult, wireupValidationResult.FlattenReasons().ToNotes("WireupError-"));
                return;
            }
            processingTimer?.StartDelayed(processingInterval, processingInterval, RunTimerProcessingCycle);
        }

        public Task Stop(CancellationToken? cancellationToken = null)
        {
            processingTimer?.Stop();
            return true.AsTask();
        }

        public async Task<OperationResult<QdActionResult[]>> RunQdActionQueueProcessingCycle()
        {
            return await RunProcessingCycle();
        }

        private async Task RunTimerProcessingCycle()
        {
            await RunProcessingCycle();
        }

        private async Task<OperationResult<QdActionResult[]>> RunProcessingCycle()
        {
            OperationResult<QdActionResult[]> result = OperationResult.Fail("Not yet started").WithoutPayload<QdActionResult[]>();

            await
                new Func<Task>(async () =>
                {
                    await logger.LogTrace($"Running QD Actions Processing cycle...");
                    using (new TimeMeasurement(async x => await logger.LogTrace($"DONE Running QD Actions Processing cycle in {x}")))
                    {
                        QdAction[] qdActionsToProcess = (await qdActionBrowser.LoadPage(new QdActionFilter
                        {
                            MaxRunCount = maxProcessingAttempts - 1,
                            Statuses = new QdActionStatus[] { QdActionStatus.Queued, QdActionStatus.Failed },
                            PageFilter = new PageFilter { PageSize = processingBatchSize }
                        }))
                        .ThrowOnFailOrReturn()
                        ?.Content;

                        if (!qdActionsToProcess?.Any() ?? true)
                        {
                            await logger.LogTrace($"There are no QD Actions to process. Skipping processing cycle...");
                            result = OperationResult.Win().WithoutPayload<QdActionResult[]>();
                            return;
                        }

                        QdActionResult[] qdActionResults = await Task.WhenAll(
                            qdActionsToProcess.Select(x => ProcessQdAction(x)).ToArray()
                        );

                        result = OperationResult.Win().WithPayload(qdActionResults);
                    }
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        await logger.LogError(ex);
                        result = OperationResult.Fail(ex, $"Error occurred while running QD Action Queue processing cycle. Message: {ex.Message}").WithoutPayload<QdActionResult[]>();
                    }
                );

            return result;
        }

        private async Task<QdActionResult> ProcessQdAction(QdAction qdAction)
        {
            QdActionResult result = OperationResult.Fail("Not yet started").WithPayload(qdAction).ToQdActionResult();

            await
                new Func<Task>(async () =>
                {
                    await logger.LogTrace($"Processing QdAction {qdAction}");
                    using (new TimeMeasurement(async x => await logger.LogTrace($"DONE Processing QdAction {qdAction} in {x}")))
                    {
                        //Check status, in case another processor already picked it up
                        qdAction = (await qdActionStorage.LoadByID(qdAction.ID)).ThrowOnFailOrReturn();
                        if (qdAction.Status.NotIn(QdActionStatus.Queued, QdActionStatus.Failed) || qdAction.RunCount >= maxProcessingAttempts)
                        {
                            await logger.LogTrace($"SKIPPING Processing QdAction {qdAction} because it has already been picked up by another agent");
                            return;
                        }

                        (await qdActionStorage.Save(qdAction.And(x => x.Status = QdActionStatus.Running))).ThrowOnFail();

                        QdActionResult processingResult = await RunEligibleProcessorForQdAction(qdAction);

                        (await qdActionStorage.Save(qdAction.And(x =>
                        {
                            x.RunCount++;
                            x.Status = processingResult.IsSuccessful ? QdActionStatus.Succeeded : QdActionStatus.Failed;
                        }))).ThrowOnFail();

                        (await qdActionResultStorage.Save(processingResult)).ThrowOnFail();

                        result = processingResult;
                    }
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        await logger.LogError(ex);
                        result = OperationResult.Fail(ex, $"Error occurred while processing QD Action {qdAction}. Message: {ex.Message}").WithPayload(qdAction).ToQdActionResult();
                    }
                );

            return result;
        }

        private async Task<QdActionResult> RunEligibleProcessorForQdAction(QdAction qdAction)
        {
            QdActionResult result = OperationResult.Fail("Not yet started").WithPayload(qdAction).ToQdActionResult();

            await
                new Func<Task>(async () =>
                {
                    ImAQdActionProcessor qdActionProcessor = await GetProcessorFor(qdAction);
                    if (qdActionProcessor == null)
                    {
                        result = OperationResult.Fail($"No eligible QD Action Processor found for {qdAction}").WithPayload(qdAction).ToQdActionResult();
                        return;
                    }

                    result = await qdActionProcessor.Process(qdAction);
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        await logger.LogError(ex);
                        result = OperationResult.Fail(ex).WithPayload(qdAction).ToQdActionResult();
                    }
                );

            return result;
        }

        private async Task<ImAQdActionProcessor> GetProcessorFor(QdAction qdAction)
        {
            if (!allKnownProcessors?.Any() ?? true)
                return null;

            foreach (ImAQdActionProcessor processor in allKnownProcessors)
            {
                if (await processor.IsEligibleFor(qdAction))
                    return processor;
            }

            return null;
        }

        private Task<OperationResult> EnsureWireup()
        {
            if (
                qdActionStorage == null
                || qdActionResultStorage == null
                || qdActionBrowser == null
            )
                return OperationResult.Fail("Storage services for QdAction and QdActionResult not found").AsTask();

            return OperationResult.Win().AsTask();
        }
    }
}
