using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime
{
    public class SyncRequestProcessingDaemon : ImADaemon, ImADependency
    {
        #region Construct
        static readonly IDentity daemonIdentity = new InternalIdentity
        {
            ID = Guid.Parse("A2660F9E-A27C-4D80-8B14-8FF3B57B4EC6"),
            IDTag = nameof(SyncRequestProcessingDaemon),
            DisplayName = "Sync Request Processing Daemon",
        };

#if DEBUG
        static readonly TimeSpan processingInterval = TimeSpan.FromSeconds(5);
#else
        static readonly TimeSpan processingInterval = TimeSpan.FromSeconds(30);
#endif
        const int processingBatchSize = 10;

        ImAPeriodicAction periodicAction;
        ImAStorageBrowserService<SyncRequest, SyncRequestFilter> syncRequestBrowserResource = null;
        ImAStorageService<string, SyncRequest> syncRequestStorageResource = null;
        ImASyncRequestProcessorFactory syncRequestProcessorFactory = null;
        ImAnAuditingService auditingService = null;
        ImALogger logger;

        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            periodicAction = dependencyProvider.Get<ImAPeriodicAction>();
            syncRequestBrowserResource = dependencyProvider.Get<ImAStorageBrowserService<SyncRequest, SyncRequestFilter>>();
            syncRequestStorageResource = dependencyProvider.Get<ImAStorageService<string, SyncRequest>>();
            syncRequestProcessorFactory = dependencyProvider.Get<ImASyncRequestProcessorFactory>();
            auditingService = dependencyProvider.Get<ImAnAuditingService>();
            logger = dependencyProvider.GetLogger<SyncRequestProcessingDaemon>("H.Necessaire.Runtime");
        }
        #endregion

        public Task Start(CancellationToken? cancellationToken = null)
        {
            periodicAction.Start(processingInterval, async () => await DoWork(cancellationToken));
            return true.AsTask();
        }

        public Task Stop(CancellationToken? cancellationToken = null)
        {
            periodicAction.Stop();
            return true.AsTask();
        }

        private async Task DoWork(CancellationToken? cancellationToken = null)
        {
            await logger.LogInfo($"Processing Sync Requests");

            using (new TimeMeasurement(x => logger.LogInfo($"DONE Processing Sync Requests in {x}").Wait()))
            {
                await
                    new Func<Task>(async () =>
                    {
                        Page<SyncRequest> requestsToProcess
                        = (await syncRequestBrowserResource.LoadPage(new SyncRequestFilter
                        {
                            SyncStatuses = new SyncStatus[] { SyncStatus.NotSynced, SyncStatus.DeletedAndNotSynced },
                        }))
                        .ThrowOnFailOrReturn();

                        if (!requestsToProcess.Content?.Any() ?? true)
                        {
                            await logger.LogInfo($"There are no sync requests to process. Skipping processing cycle...");
                            return;
                        }

                        await logger.LogInfo($"Found a batch of {requestsToProcess.Content.Length} requests to process. Types: {string.Join(", ", requestsToProcess.Content.Select(x => x.PayloadType).Distinct())}");

                        foreach (SyncRequest syncRequest in requestsToProcess.Content)
                        {
                            await ProcessSyncRequest(syncRequest);
                        }
                    })
                    .TryOrFailWithGrace(
                        onFail: async ex => await logger.LogError($"An error occured while processing the sync requests. Details below.{Environment.NewLine}{ex}{Environment.NewLine}")
                    );
            }
        }

        private async Task<OperationResult<SyncRequest>> ProcessSyncRequest(SyncRequest syncRequest)
        {
            OperationResult<SyncRequest> result = OperationResult.Win().WithPayload(syncRequest);

            await
                new Func<Task>(async () =>
                {
                    ImASyncRequestProcessor syncRequestProcessor = await syncRequestProcessorFactory.BuildProcessorFor(syncRequest);

                    if (syncRequestProcessor == null)
                    {
                        await logger.LogInfo($"Cannot find any processor for Sync Request of type: {syncRequest.PayloadType}");
                        return;
                    }

                    await logger.LogInfo($"Processing {syncRequest}...");
                    using (new TimeMeasurement(x => logger.LogInfo($"DONE Processing {syncRequest} in {x}").Wait()))
                    {
                        OperationResult processingResult
                            = syncRequestProcessor is ImASyncRequestExilationProcessor
                            ? await (syncRequestProcessor as ImASyncRequestExilationProcessor).ProcessExilation(syncRequest, OperationResult.Win())
                            : await syncRequestProcessor.Process(syncRequest)
                            ;

                        if (!processingResult.IsSuccessful)
                        {
                            await logger.LogError($"Processing {syncRequest} failed because: {string.Join(Environment.NewLine, processingResult.FlattenReasons())}");
                            result = processingResult.WithPayload(syncRequest);
                            return;
                        }

                        await auditingService.Append(syncRequest.ToAuditMeta<SyncRequest, string>(AuditActionType.Remove, daemonIdentity), syncRequest);

                        await syncRequestStorageResource.DeleteByID(syncRequest.ID);
                    }

                })
                .TryOrFailWithGrace(
                    onFail: ex => result = OperationResult.Fail(ex).WithPayload(syncRequest)
                );

            if (result.IsSuccessful)
                return result;

            await logger.LogInfo($"Sync Request Processing failed. Will exile sync request {syncRequest}");
            await
                new Func<Task>(async () =>
                {
                    ImASyncRequestExilationProcessor syncRequestExilationProcessor = await syncRequestProcessorFactory.BuildExilationProcessor();

                    if (syncRequestExilationProcessor == null)
                    {
                        await logger.LogWarn($"Cannot find any exilation processor");
                        return;
                    }

                    await logger.LogInfo($"Exiling {syncRequest}...");
                    using (new TimeMeasurement(x => logger.LogInfo($"DONE Exiling {syncRequest} in {x}").Wait()))
                    {
                        OperationResult processingResult = await syncRequestExilationProcessor.ProcessExilation(syncRequest, result.WithoutPayload<SyncRequest>());
                        if (!processingResult.IsSuccessful)
                        {
                            await logger.LogError($"Exiling {syncRequest} failed because: {string.Join(Environment.NewLine, processingResult.FlattenReasons())}");
                            result = processingResult.WithPayload(syncRequest);
                            return;
                        }

                        await auditingService.Append(syncRequest.ToAuditMeta<SyncRequest, string>(AuditActionType.Remove, daemonIdentity), syncRequest);

                        await syncRequestStorageResource.DeleteByID(syncRequest.ID);
                    }

                })
                .TryOrFailWithGrace(
                    onFail: ex => result = OperationResult.Fail(ex).WithPayload(syncRequest)
                );

            return result;
        }
    }
}
