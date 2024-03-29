﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class SyncDaemon : WebWorkerDaemonBase
    {
        public SyncDaemon(Func<ImAnAppWireup> webWorkerWireup = null, params string[] scriptsToInclude)
            : base(
                () => AppBase.Get<Worker>(),
                webWorkerWireup,
                scriptsToInclude
            )
        {
        }

        public class Worker : ImAWebWorkerDaemonAction, ImADependency
        {
            ActionRepeater repeater;

            ImASyncableBrowser syncablesBrowser;
            ImASyncRegistry[] syncRegistries;
            SyncRequestResource syncRequestResource;
            ConsumerIdentityLocalStorageResource consumerIdentityStorage;
            ImALogger logger;
            public void ReferDependencies(ImADependencyProvider dependencyProvider)
            {
                repeater = new ActionRepeater(RunSyncCycle, TimeSpan.FromSeconds(AppBase.Config.Get("SyncIntervalInSeconds")?.ToString()?.ParseToIntOrFallbackTo(10).Value ?? 10));
                syncablesBrowser = dependencyProvider.Get<ImASyncableBrowser>();
                syncRegistries = dependencyProvider.Get<ImASyncRegistry[]>() ?? dependencyProvider.Get<ImASyncRegistry>()?.AsArray() ?? new ImASyncRegistry[0];
                syncRequestResource = dependencyProvider.Get<SyncRequestResource>();
                consumerIdentityStorage = dependencyProvider.Get<ConsumerIdentityLocalStorageResource>();
                logger = dependencyProvider.GetLogger<Worker>();
            }

            public async void DoWork()
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                await repeater.Start();
            }

            private async Task RunSyncCycle()
            {
                if (!AppBase.IsOnline)
                {
                    await logger.LogTrace($"Skipping Sync Cycle because we're offline");
                    return;
                }

                await logger.LogDebug($"Running Sync Cycle");
                using (new TimeMeasurement(async x => await logger.LogTrace($"DONE Running Sync Cycle in {x}")))
                {
                    Type[] syncableTypes = await syncablesBrowser.GetAllSyncableTypes();

                    await logger.LogTrace($"Syncable types: {string.Join(", ", syncableTypes.Select(x => x.TypeName()).ToArray())}");

                    foreach (ImASyncRegistry syncRegistry in syncRegistries)
                    {
                        await RunSyncCycleForSyncRegistry(syncRegistry, syncableTypes);
                    }
                }
            }

            private async Task RunSyncCycleForSyncRegistry(ImASyncRegistry syncRegistry, Type[] syncableTypes)
            {
                await
                    new Func<Task>(async () =>
                    {
                        await logger.LogTrace($"Syncing batch for {syncRegistry.ReceiverNode}");
                        using (new TimeMeasurement(async x => await logger.LogTrace($"DONE Syncing batch for {syncRegistry.ReceiverNode} in {x}")))
                        {
                            foreach (Type syncableType in syncableTypes)
                            {
                                string[] entitiesToSync = await syncRegistry.GetEntitiesToSync(syncableType.TypeName());

                                if (!entitiesToSync?.Any() ?? true)
                                {
                                    await logger.LogTrace($"Nothing to sync for {syncableType.TypeName()}. Skipping...");
                                    continue;
                                }

                                await logger.LogTrace($"Found {entitiesToSync.Length} entries to sync for {syncableType.TypeName()}");

                                List<SyncRequest> syncRequests = new List<SyncRequest>(entitiesToSync.Length);

                                foreach (string entityID in entitiesToSync)
                                {
                                    object actualEntity = (await syncablesBrowser.LoadEntity(syncableType, entityID));
                                    syncRequests.Add(new SyncRequest
                                    {
                                        Payload = actualEntity == null ? null : Newtonsoft.Json.JsonConvert.SerializeObject(actualEntity.ObjectToJson()),
                                        PayloadIdentifier = entityID,
                                        PayloadType = syncableType.TypeName(),
                                        SyncStatus = await syncRegistry.StatusFor(syncableType.TypeName(), entityID),
                                        OperationContext = new OperationContext
                                        {
                                            Consumer = await consumerIdentityStorage.Search(),
                                        },
                                    });
                                }

                                await logger.LogTrace($"Sync Requests for {syncableType.TypeName()}", syncRequests.ObjectToJson());

                                OperationResult<SyncResponse>[] syncResults = await syncRequestResource.Sync(syncRegistry.ReceiverNode, syncRequests.ToArray());

                                foreach (OperationResult<SyncResponse> syncReponse in syncResults)
                                {
                                    if (!syncReponse.IsSuccessful)
                                        continue;

                                    await syncRegistry.SetStatusFor(syncReponse.Payload.PayloadType, syncReponse.Payload.PayloadIdentifier, syncReponse.Payload.SyncStatus);
                                }
                            }
                        }
                    })
                    .TryOrFailWithGrace(onFail: async x => await logger.LogError(x))
                    ;

            }
        }
    }
}
