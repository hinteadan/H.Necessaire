using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace H.Necessaire.Operations.Caching.Concrete
{
    internal class InMemoryCacher<T> : ImACacher<T>
    {
        readonly ConcurrentDictionary<string, ImCachebale<T>> cacheRegistry = new ConcurrentDictionary<string, ImCachebale<T>>();

        public Task<T> GetOrAdd(string id, Func<string, Task<ImCachebale<T>>> cacheableItemFactory)
        {
            throw new NotImplementedException();
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
