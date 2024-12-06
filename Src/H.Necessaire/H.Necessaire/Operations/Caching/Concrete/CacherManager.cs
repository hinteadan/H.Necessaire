using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace H.Necessaire.Operations.Caching.Concrete
{
    internal class CacherManager : ImACacherFactory, ImACacherRegistry, ImADependency
    {
#if DEBUG
        TimeSpan housekeepingInterval = TimeSpan.FromSeconds(10);
#else
        TimeSpan housekeepingInterval = TimeSpan.FromMinutes(1);
#endif
        ImAPeriodicAction housekeeping;
        ImADependencyProvider dependencyProvider;
        readonly ConcurrentDictionary<Type, ImACacher> cacherRegistry = new ConcurrentDictionary<Type, ImACacher>();
        ImALogger logger;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            this.dependencyProvider = dependencyProvider;
            housekeeping = dependencyProvider.Get<ImAPeriodicAction>();
            logger = dependencyProvider.GetLogger(nameof(CacherManager));
            double? housekeepingIntervalFromConfig = dependencyProvider.GetRuntimeConfig()?.Get("CachingHousekeepingIntervalInSeconds")?.ToString()?.ParseToDoubleOrFallbackTo(null);
            if (housekeepingIntervalFromConfig != null)
                housekeepingInterval = TimeSpan.FromSeconds(housekeepingIntervalFromConfig.Value);
        }

        public ImACacher<T> BuildCacher<T>(string cacherID = "InMemory")
        {
            ImACacher existingCacher = null;
            if (cacherRegistry.TryGetValue(typeof(ImACacher<T>), out existingCacher))
                return existingCacher as ImACacher<T>;

            ImACacher<T> cacher =
                (cacherID.IsEmpty()
                ? dependencyProvider?.Get<ImACacher<T>>()
                : dependencyProvider?.Build<ImACacher<T>>(cacherID, dependencyProvider?.Get<ImACacher<T>>())
                )
                ??
                BuildNewInMemoryCacher<T>()
                ;

            if (cacher == null)
                return null;

            cacherRegistry.TryAdd(typeof(ImACacher<T>), cacher);

            housekeeping.StartDelayed(housekeepingInterval, housekeepingInterval, RunHousekeepingSession);

            return cacher;
        }

        private ImACacher<T> BuildNewInMemoryCacher<T>()
        {
            return
                new InMemoryCacher<T>();
        }

        private async Task RunHousekeepingSession()
        {
            foreach (ImACacher cacher in cacherRegistry.Values)
            {
                await RunHousekeepingSessionFor(cacher);
            }
        }

        private async Task RunHousekeepingSessionFor(ImACacher cacher)
        {
            await
                new Func<Task>(async () =>
                {

                    await cacher.RunHousekeepingSession();

                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        await logger.LogError($"Error occurred while trying to Run Housekeeping Session For {cacher?.GetType()?.Name}. Message: {ex.Message}", ex);
                    }
                );
        }

        public async Task ClearAll()
        {
            if (cacherRegistry.Count == 0)
                return;

            foreach (ImACacher cacher in cacherRegistry.Values)
            {
                await cacher.ClearAll();
            }
        }

        public async Task Clear(params string[] ids)
        {
            if (cacherRegistry.Count == 0)
                return;

            foreach (ImACacher cacher in cacherRegistry.Values)
            {
                await cacher.Clear(ids);
            }
        }
    }
}
