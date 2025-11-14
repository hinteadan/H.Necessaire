
using H.Necessaire.Runtime.UI.Razor.Core.Managers;

namespace H.Necessaire.Runtime.UI.Razor.Core.UseCases
{
    internal class ConsumerUseCase : UseCaseBase, ImAConsumerUseCase
    {
        static readonly TimeSpan consumerIdentityDetailsTimeout = TimeSpan.FromMinutes(5);
        ConsumerManager consumerManager;
        Func<HJs> hjsProvider;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            hjsProvider = dependencyProvider.Get<Func<HJs>>();
            consumerManager = dependencyProvider.Get<ConsumerManager>();
        }

        public async Task<ConsumerIdentity> CreateOrResurrect()
        {
            ConsumerIdentity consumerIdentity
                = await consumerManager.GetCurrentConsumer()
                ?? await hjsProvider().GetConsumerInfo(Guid.NewGuid())
                ;

            if (consumerIdentity.AsOf + consumerIdentityDetailsTimeout <= DateTime.UtcNow)
            {
                consumerIdentity = await hjsProvider().GetConsumerInfo(consumerIdentity.ID);
            }

            await consumerManager.SetCurrentConsumer(consumerIdentity);

            return consumerIdentity;
        }

        public async Task<ConsumerIdentity> Resurrect()
        {
            return await consumerManager.GetCurrentConsumer();
        }
    }
}
