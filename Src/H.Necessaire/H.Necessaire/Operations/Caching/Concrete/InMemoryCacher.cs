using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace H.Necessaire.Operations.Caching.Concrete
{
    internal class InMemoryCacher<T> : ImACacher<T>
    {
        readonly ConcurrentDictionary<string, ImCachebale<T>> cacheRegistry = new ConcurrentDictionary<string, ImCachebale<T>>();

        public async Task<T> GetOrAdd(string id, Func<string, Task<ImCachebale<T>>> cacheableItemFactory)
        {
            DateTime now = DateTime.UtcNow;
            ImCachebale<T> cachedItem = null;
            if(cacheRegistry.TryGetValue(id, out cachedItem) && !cachedItem.IsExpired(now))
            {                
                cachedItem.MarkAccess(now);
                if (!cachedItem.IsSlidingExpirationDisabled && cachedItem.ValidFor != null)
                {
                    cachedItem.ActiveAsOf(now);
                }

                return cachedItem.Payload;
            }

            cachedItem = await cacheableItemFactory(id);
            if(cachedItem == null)
                OperationResult.Fail($"Cacheable item {id} built via external cacheableItemFactory is NULL").ThrowOnFail();

            cacheRegistry.AddOrUpdate(id, cachedItem, (key, existing) => cachedItem);

            return cachedItem.Payload;
        }

        public Task ClearAll()
        {
            throw new NotImplementedException();
        }

        public Task RunHousekeepingSession()
        {
            throw new NotImplementedException();
        }
    }
}
