using H.Necessaire.Runtime.UI.Razor.Core.Storage;
using Tavenem.Blazor.IndexedDB;

namespace H.Necessaire.Runtime.UI.Razor.Core.Managers
{
    internal class ConsumerManager : ImADependency, ImADependencyGroup
    {
        static ConsumerIdentity currentConsumerIdentity = null;
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry.Register<Func<ConsumerIdentity>>(() => () => currentConsumerIdentity);
        }
        Func<HIndexedDbContext> hIndexedDbContextProvider;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            hIndexedDbContextProvider = dependencyProvider.Get<Func<HIndexedDbContext>>();
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
                //Save to Consumer Storage
                //Save to Audits
                //These depend on the DI registrations for the ImAStorgeService<X, X>
                //Can be either stored locally in IndexedDB and sync-ed in the future or POST-ed to the server directly
            }
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
            await foreach (ConsumerIdentity consumerIdentity in allConsumers)
            {
                if (consumerIdentity.AsOf >= (currentConsumerIdentity?.AsOf ?? DateTime.MinValue))
                    currentConsumerIdentity = consumerIdentity;
            }
            return currentConsumerIdentity;
        }

        
    }
}
