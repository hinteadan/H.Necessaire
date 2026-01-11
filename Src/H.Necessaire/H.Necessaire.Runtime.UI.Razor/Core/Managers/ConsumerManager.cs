using H.Necessaire.Runtime.UI.Razor.Core.Storage;
using System.Runtime.InteropServices;
using Tavenem.Blazor.IndexedDB;

namespace H.Necessaire.Runtime.UI.Razor.Core.Managers
{
    internal class ConsumerManager : ImADependency
    {
        ConsumerIdentity currentConsumerIdentity = null;
        Func<HIndexedDbContext> hIndexedDbContextProvider;
        ImAStorageService<Guid, ConsumerIdentity> consumerIdentityStorageService;
        ImAnAuditingService auditingService;
        ImAnActionQer actionQer;
        ImALogger log;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            hIndexedDbContextProvider = dependencyProvider.Get<Func<HIndexedDbContext>>();
            consumerIdentityStorageService = dependencyProvider.Get<ImAStorageService<Guid, ConsumerIdentity>>();
            auditingService = dependencyProvider.Get<ImAnAuditingService>();
            actionQer = dependencyProvider.Get<ImAnActionQer>();
            log = dependencyProvider.GetLogger<ConsumerManager>();
        }

        public async Task SetCurrentConsumer(ConsumerIdentity consumerIdentity)
        {
            if (consumerIdentity == currentConsumerIdentity)
                return;

            currentConsumerIdentity = consumerIdentity;
            HIndexedDbContext dbContext = hIndexedDbContextProvider();
            IndexedDbStore consumerIdentityStore = dbContext.CoreDatabase[nameof(ConsumerIdentity)];
            bool isOK = await consumerIdentityStore.StoreAsync(consumerIdentity);

            if (isOK)
            {
                await consumerIdentityStorageService.Save(consumerIdentity);//This will trigger audit and QdAction for WASM
                if (RuntimeInformation.ProcessArchitecture != Architecture.Wasm)
                {
                    await AuditConsumerIfNecessary(consumerIdentity);
                    await QueueConsumerDetailsProcessingIfNecessary(consumerIdentity);
                }
            }
        }

        async Task QueueConsumerDetailsProcessingIfNecessary(ConsumerIdentity consumerIdentity)
        {
            if (actionQer is null)
                return;

            if (consumerIdentity is null)
                return;

            await HSafe.Run(async () =>
            {

                if (!consumerIdentity.IpAddress.IsEmpty())
                    await actionQer.Queue(QdAction.New(WellKnown.QdActionType.ProcessIpAddress, WellKnown.QdActionType.ProcessIpAddressPayload(consumerIdentity))).LogError(log, "actionQer.Queue ProcessIpAddress");

                if (consumerIdentity.RuntimePlatform != null)
                    await actionQer.Queue(QdAction.New(WellKnown.QdActionType.ProcessRuntimePlatform, WellKnown.QdActionType.ProcessRuntimePlatformPayload(consumerIdentity))).LogError(log, "actionQer.Queue ProcessRuntimePlatform");

            }, "QueueConsumerDetailsProcessingIfNecessary")
            .LogError(log, "QueueConsumerDetailsProcessingIfNecessary");
        }

        async Task AuditConsumerIfNecessary(ConsumerIdentity consumerIdentity)
        {
            if (auditingService is null)
                return;

            if (consumerIdentity is null)
                return;

            await HSafe.Run(async () =>
            {

                await auditingService.Append(
                    consumerIdentity.ToAuditMeta(consumerIdentity.ID.ToString(), AuditActionType.Create),
                    consumerIdentity
                );

            });
        }

        public async Task<ConsumerIdentity> GetCurrentConsumer()
        {
            currentConsumerIdentity ??= await Resurrect();
            return currentConsumerIdentity;
        }

        async Task<ConsumerIdentity> Resurrect()
        {
            HIndexedDbContext dbContext = hIndexedDbContextProvider();
            IndexedDbStore consumerIdentityStore = dbContext.CoreDatabase[nameof(ConsumerIdentity)];
            IAsyncEnumerable<ConsumerIdentity> allConsumers = consumerIdentityStore.GetAllAsync<ConsumerIdentity>();
            ConsumerIdentity consumerIdentity = await allConsumers.OrderByDescending(x => x.AsOf).FirstOrDefaultAsync();
            return consumerIdentity;
        }
    }
}
