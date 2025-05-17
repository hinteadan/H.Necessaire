using H.Necessaire.Serialization;

namespace H.Necessaire.Runtime.MAUI.Core
{
    class ConsumerIdentityManager : ImAConsumerUseCase, ImADependency, ImADependencyGroup
    {
        ConsumerIdentity consumerIdentity;
        string storageKey = "H-Necessaire-ConsumerIdentity";
        ISecureStorage secureStorage;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            secureStorage = SecureStorage.Default;
        }
        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry.RegisterAlwaysNew<ConsumerIdentity>(() => consumerIdentity);
        }

        public async Task<ConsumerIdentity> CreateOrResurrect()
        {
            consumerIdentity = null;

            string existingConsumerIdentiyJson = await secureStorage.GetAsync(storageKey);

            if (!existingConsumerIdentiyJson.IsEmpty())
                consumerIdentity = existingConsumerIdentiyJson.JsonToObject<ConsumerIdentity>();

            consumerIdentity ??= new ConsumerIdentity();

            consumerIdentity = consumerIdentity.DecorateWithDeviceRuntimeInfo();

            await secureStorage.SetAsync(storageKey, consumerIdentity.ToJsonObject());

            return consumerIdentity;
        }

        public async Task<ConsumerIdentity> Resurrect()
        {
            consumerIdentity = null;

            string existingConsumerIdentiyJson = await secureStorage.GetAsync(storageKey);

            if (!existingConsumerIdentiyJson.IsEmpty())
                consumerIdentity = existingConsumerIdentiyJson.JsonToObject<ConsumerIdentity>();

            if (consumerIdentity is null)
                return null;

            consumerIdentity = consumerIdentity.DecorateWithDeviceRuntimeInfo();

            await secureStorage.SetAsync(storageKey, consumerIdentity.ToJsonObject());

            return consumerIdentity;
        }
    }
}
