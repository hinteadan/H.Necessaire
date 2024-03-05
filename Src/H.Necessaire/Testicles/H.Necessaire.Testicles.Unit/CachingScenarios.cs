using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace H.Necessaire.Testicles.Unit
{
    public class CachingScenarios
    {
        readonly ImADependencyRegistry dependencyRegistry = IoC.NewDependencyRegistry();
        public CachingScenarios() 
        {
            dependencyRegistry
                .Register<HNecessaireDependencyGroup>(() => new HNecessaireDependencyGroup())
                ;
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
        }
    }
}
