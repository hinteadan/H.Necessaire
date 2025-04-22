using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace H.Necessaire.Testicles.Unit
{
    public class CachingScenarios
    {
        static readonly TimeSpan housekeepingInterval = TimeSpan.FromSeconds(1);
        readonly ImADependencyRegistry dependencyRegistry = IoC.NewDependencyRegistry();
        public CachingScenarios() 
        {
            dependencyRegistry
                .Register<HNecessaireDependencyGroup>(() => new HNecessaireDependencyGroup())
                ;
            RuntimeConfig runtimeConfig = dependencyRegistry.GetRuntimeConfig();
            runtimeConfig.Values = runtimeConfig.Values.Push(new ConfigNode[] {
                "CachingHousekeepingIntervalInSeconds".ConfigWith(.2d.ToString())
            });
        }

        [Fact(DisplayName = "Caching Retrieves Data From Cache If Possible")]
        public async Task Caching_Retrieves_Data_From_Cache_If_Possible()
        {
            ImACacher<int> cacher = dependencyRegistry.GetCacher<int>();

            int value = 0;
            int cachedValue = await cacher.GetOrAdd("IntCacheTest", id => value.ToCacheableItem(id).AsTask());
            cachedValue.Should().Be(0, because: "0 is the value added to cache");

            value = -1;
            cachedValue = await cacher.GetOrAdd("IntCacheTest", id => value.ToCacheableItem(id).AsTask());
            cachedValue.Should().Be(0, because: "we cached the initial value of 0, not the -1");

            await cacher.Clear("IntCacheTest");

            cachedValue = await cacher.GetOrAdd("IntCacheTest", id => value.ToCacheableItem(id).AsTask());
            cachedValue.Should().Be(-1, because: "we cleared the cached item and added the new value");

            value = -10;
            await cacher.ClearAll();
            cachedValue = await cacher.GetOrAdd("IntCacheTest", id => value.ToCacheableItem(id).AsTask());
            cachedValue.Should().Be(-10, because: "we cleared the entire cache and added the new value");
        }

        [Fact(DisplayName = "Caching Correctly Handles Cached Items With Sliding Expiration Policy")]
        public async Task Caching_Correctly_Handles_Cached_Items_With_Sliding_Expiration_Policy()
        {
            ImACacher<int> cacher = dependencyRegistry.GetCacher<int>();

            int value = 42;
            await cacher.GetOrAdd("IntCacheTest", id => value.ToCacheableItem(id, cacheDuration: TimeSpan.FromSeconds(.8)).AsTask());
            int cachedValue = (await cacher.TryGet("IntCacheTest")).ThrowOnFailOrReturn();
            cachedValue.Should().Be(42, because: "we just cached the value 42 for 1 second");

            await Task.Delay(TimeSpan.FromSeconds(.4));
            cachedValue = (await cacher.TryGet("IntCacheTest")).ThrowOnFailOrReturn();
            cachedValue.Should().Be(42, because: "The cached value of 42 hasn't expired yet");

            await Task.Delay(TimeSpan.FromSeconds(.65));
            cachedValue = (await cacher.TryGet("IntCacheTest")).ThrowOnFailOrReturn();
            cachedValue.Should().Be(42, because: "The cached value of 42 hasn't expired yet because it was slided");

            await Task.Delay(TimeSpan.FromSeconds(.9));

            OperationResult cachedValueResult = await cacher.TryGet("IntCacheTest");
            cachedValueResult.IsSuccessful.Should().BeFalse(because: "the cached item should have been removed from cache during a housekeeping session as it was not accessed and therefore not slided");
        }

        [Fact(DisplayName = "Caching Correctly Handles Cached Items With NO Sliding Expiration Policy")]
        public async Task Caching_Correctly_Handles_Cached_Items_With_NO_Sliding_Expiration_Policy()
        {
            ImACacher<int> cacher = dependencyRegistry.GetCacher<int>();

            int value = 42;
            await cacher.GetOrAdd("IntCacheTest", id => value.ToCacheableItem(id, cacheDuration: TimeSpan.FromSeconds(.8)).And(x => x.IsSlidingExpirationDisabled = true).AsTask());
            int cachedValue = (await cacher.TryGet("IntCacheTest")).ThrowOnFailOrReturn();
            cachedValue.Should().Be(42, because: "we just cached the value 42 for 1 second");

            await Task.Delay(TimeSpan.FromSeconds(.4));
            cachedValue = (await cacher.TryGet("IntCacheTest")).ThrowOnFailOrReturn();
            cachedValue.Should().Be(42, because: "The cached value of 42 hasn't expired yet");

            await Task.Delay(TimeSpan.FromSeconds(.65));
            OperationResult cachedValueResult = await cacher.TryGet("IntCacheTest");
            cachedValueResult.IsSuccessful.Should().BeFalse(because: "the cached item should have been removed from cache during a housekeeping session as even though it was accessed, it has sliding expiration disabled");
        }

        [Fact(DisplayName = "Caching Correctly Updates Cached Items")]
        public async Task Caching_Correctly_Updates_Cached_Items()
        {
            ImACacher<int> cacher = dependencyRegistry.GetCacher<int>();

            int value = 42;
            await cacher.GetOrAdd("IntCacheTest", id => value.ToCacheableItem(id).AsTask());
            int cachedValue = (await cacher.TryGet("IntCacheTest")).ThrowOnFailOrReturn();
            cachedValue.Should().Be(42, because: "we just cached the value 42");

            value = 43;
            await cacher.AddOrUpdate("IntCacheTest", id => value.ToCacheableItem(id).AsTask());
            cachedValue = (await cacher.TryGet("IntCacheTest")).ThrowOnFailOrReturn();
            cachedValue.Should().Be(43, because: "we just updated the cached value to 43");
        }
    }
}
