using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime
{
    public class SyncRequestProcessingDaemon : ImADaemon, ImADependency
    {
        #region Construct
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

        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            periodicAction = dependencyProvider.Get<ImAPeriodicAction>();
            syncRequestBrowserResource = dependencyProvider.Get<ImAStorageBrowserService<SyncRequest, SyncRequestFilter>>();
            syncRequestStorageResource = dependencyProvider.Get<ImAStorageService<string, SyncRequest>>();
            syncRequestProcessorFactory = dependencyProvider.Get<ImASyncRequestProcessorFactory>();
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
            Console.WriteLine($"{DateTime.Now} Processing Sync Requests");

            using (new TimeMeasurement(x => Console.WriteLine($"{DateTime.Now} DONE Processing Sync Requests in {x}")))
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
                            Console.WriteLine($"{DateTime.Now} There are no sync requests to process. Skipping processing cycle...");
                            return;
                        }

                        Console.WriteLine($"{DateTime.Now} Found a batch of {requestsToProcess.Content.Length} requests to process. Types: {string.Join(", ", requestsToProcess.Content.Select(x => x.PayloadType).Distinct())}");

                        foreach (SyncRequest syncRequest in requestsToProcess.Content)
                        {
                            await ProcessSyncRequest(syncRequest);
                        }
                    })
                    .TryOrFailWithGrace(
                        onFail: ex => Console.WriteLine($"{DateTime.Now} An error occured while processing the sync requests. Details below.{Environment.NewLine}{ex}{Environment.NewLine}")
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
                        Console.WriteLine($"{DateTime.Now} Cannot find any processor for Sync Request of type: {syncRequest.PayloadType}");
                        return;
                    }

                    Console.WriteLine($"{DateTime.Now} Processing {syncRequest}...");
                    using (new TimeMeasurement(x => Console.WriteLine($"{DateTime.Now} DONE Processing {syncRequest} in {x}")))
                    {
                        OperationResult processingResult
                            = syncRequestProcessor is ImASyncRequestExilationProcessor
                            ? await (syncRequestProcessor as ImASyncRequestExilationProcessor).ProcessExilation(syncRequest, OperationResult.Win())
                            : await syncRequestProcessor.Process(syncRequest)
                            ;

                        if (!processingResult.IsSuccessful)
                        {
                            Console.WriteLine($"{DateTime.Now} Processing {syncRequest} failed because: {string.Join(Environment.NewLine, processingResult.FlattenReasons())}");
                            result = processingResult.WithPayload(syncRequest);
                            return;
                        }

                        await syncRequestStorageResource.DeleteByID(syncRequest.ID);
                    }

                })
                .TryOrFailWithGrace(
                    onFail: ex => result = OperationResult.Fail(ex).WithPayload(syncRequest)
                );

            if (result.IsSuccessful)
                return result;

            Console.WriteLine($"{DateTime.Now} Sync Request Processing failed. Will exile sync request {syncRequest}");
            await
                new Func<Task>(async () =>
                {
                    ImASyncRequestExilationProcessor syncRequestExilationProcessor = await syncRequestProcessorFactory.BuildExilationProcessor();

                    if (syncRequestExilationProcessor == null)
                    {
                        Console.WriteLine($"{DateTime.Now} Cannot find any exilation processor");
                        return;
                    }

                    Console.WriteLine($"{DateTime.Now} Exiling {syncRequest}...");
                    using (new TimeMeasurement(x => Console.WriteLine($"{DateTime.Now} DONE Exiling {syncRequest} in {x}")))
                    {
                        OperationResult processingResult = await syncRequestExilationProcessor.ProcessExilation(syncRequest, result.WithoutPayload<SyncRequest>());
                        if (!processingResult.IsSuccessful)
                        {
                            Console.WriteLine($"{DateTime.Now} Exiling {syncRequest} failed because: {string.Join(Environment.NewLine, processingResult.FlattenReasons())}");
                            result = processingResult.WithPayload(syncRequest);
                            return;
                        }

                        //TODO: Audit the SyncRequest before the deleting

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
