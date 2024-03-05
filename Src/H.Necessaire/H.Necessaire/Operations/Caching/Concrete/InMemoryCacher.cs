using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Operations.Caching.Concrete
{
    public class InMemoryCacher<T> : ImACacher<T>
    {
        protected readonly ConcurrentDictionary<string, ImCachebale<T>> cacheRegistry = new ConcurrentDictionary<string, ImCachebale<T>>();

        public virtual async Task<T> GetOrAdd(string id, Func<string, Task<ImCachebale<T>>> cacheableItemFactory)
        {
            if(id.IsEmpty())
                OperationResult.Fail($"ID cannot be empty").ThrowOnFail();

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

        public virtual Task RunHousekeepingSession()
        {
            DateTime now = DateTime.UtcNow;

            ImCachebale<T>[] expiredItems
                = cacheRegistry
                .Values
                .Where(x => x.IsExpired(now))
                .ToArray();

            if (expiredItems.Any() != true)
                return true.AsTask();

            foreach(ImCachebale<T> expiredItem in expiredItems)
            {
                ImCachebale<T> removedItem = null;
                cacheRegistry.TryRemove(expiredItem.ID, out removedItem);
            }

            return true.AsTask();
        }

        public virtual Task ClearAll()
        {
            cacheRegistry.Clear();
            return true.AsTask();
        }

        public virtual Task Clear(params string[] ids)
        {
            if (ids?.Any() != true)
                return true.AsTask();

            foreach(string id in ids)
            {
                if (id.IsEmpty())
                    continue;

                ImCachebale<T> removedItem = null;
                cacheRegistry.TryRemove(id, out removedItem);
            }

            return true.AsTask();
        }
    }
}
