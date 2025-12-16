using H.Necessaire.Runtime.UI.Razor.Core.Storage;
using Tavenem.Blazor.IndexedDB;

namespace H.Necessaire.Runtime.UI.Razor.Core.Managers
{
    internal class ConsumerManager : ImADependency
    {
        ConsumerIdentity currentConsumerIdentity = null;
        Func<HIndexedDbContext> hIndexedDbContextProvider;
        ImAStorageService<Guid, ConsumerIdentity> consumerIdentityStorageService;
        ImAnAuditingService auditingService;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            hIndexedDbContextProvider = dependencyProvider.Get<Func<HIndexedDbContext>>();
            consumerIdentityStorageService = dependencyProvider.Get<ImAStorageService<Guid, ConsumerIdentity>>();
            auditingService = dependencyProvider.Get<ImAnAuditingService>();
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
                await consumerIdentityStorageService.Save(consumerIdentity);
                await AuditConsumerIfNecessary(consumerIdentity);
            }
        }

        async Task AuditConsumerIfNecessary(ConsumerIdentity consumerIdentity)
        {
            if (auditingService is null)
                return;

            if (consumerIdentity is null)
                return;

            await HSafe.Run(async () => {

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
