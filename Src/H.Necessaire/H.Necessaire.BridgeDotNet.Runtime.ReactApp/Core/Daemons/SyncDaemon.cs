using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class SyncDaemon : WebWorkerDaemonBase
    {
        public SyncDaemon()
            : base(
                () => AppBase.Get<Worker>()
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
            public void ReferDependencies(ImADependencyProvider dependencyProvider)
            {
                repeater = new ActionRepeater(RunSyncCycle, TimeSpan.FromSeconds(AppBase.Config.Get("SyncIntervalInSeconds")?.ToString()?.ParseToIntOrFallbackTo(10).Value ?? 10));
                syncablesBrowser = dependencyProvider.Get<ImASyncableBrowser>();
                syncRegistries = dependencyProvider.Get<ImASyncRegistry[]>() ?? dependencyProvider.Get<ImASyncRegistry>()?.AsArray() ?? new ImASyncRegistry[0];
                syncRequestResource = dependencyProvider.Get<SyncRequestResource>();
                consumerIdentityStorage = dependencyProvider.Get<ConsumerIdentityLocalStorageResource>();
            }

            public async void DoWork()
            {
                await repeater.Start();
            }

            private async Task RunSyncCycle()
            {
                if (!AppBase.IsOnline)
                {
                    Console.WriteLine($"Skipping Sync Cycle because we're offline");
                    return;
                }

                Console.WriteLine($"Running Sync Cycle");
                using (new TimeMeasurement(x => Console.WriteLine($"DONE Running Sync Cycle in {x}")))
                {
                    Type[] syncableTypes = await syncablesBrowser.GetAllSyncableTypes();

                    Console.WriteLine($"Syncable types: {string.Join(", ", syncableTypes.Select(x => x.TypeName()).ToArray())}");

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
                        Console.WriteLine($"Syncing batch for {syncRegistry.ReceiverNode}");
                        using (new TimeMeasurement(x => Console.WriteLine($"DONE Syncing batch for {syncRegistry.ReceiverNode} in {x}")))
                        {
                            foreach (Type syncableType in syncableTypes)
                            {
                                string[] entitiesToSync = await syncRegistry.GetEntitiesToSync(syncableType.TypeName());

                                if (!entitiesToSync?.Any() ?? true)
                                {
                                    Console.WriteLine($"Nothing to sync for {syncableType.TypeName()}. Skipping...");
                                    return;
                                }

                                Console.WriteLine($"Found {entitiesToSync.Length} entries to sync for {syncableType.TypeName()}");

                                List<SyncRequest> syncRequests = new List<SyncRequest>(entitiesToSync.Length);

                                foreach (string entityID in entitiesToSync)
                                {
                                    object actualEntity = (await syncablesBrowser.LoadEntity(syncableType, entityID));
                                    syncRequests.Add(new SyncRequest
                                    {
                                        Payload = Newtonsoft.Json.JsonConvert.SerializeObject(actualEntity.ObjectToJson()),
                                        PayloadIdentifier = entityID,
                                        PayloadType = syncableType.TypeName(),
                                        SyncStatus = await syncRegistry.StatusFor(syncableType.TypeName(), entityID),
                                        OperationContext = new OperationContext
                                        {
                                            Consumer = await consumerIdentityStorage.Search(),
                                        },
                                    });
                                }

                                Console.WriteLine($"Sync Requests for {syncableType.TypeName()}");
                                Console.WriteLine(syncRequests.ObjectToJson());

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
                    .TryOrFailWithGrace(onFail: x => Console.WriteLine(x))
                    ;

            }
        }
    }
}
