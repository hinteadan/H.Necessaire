using System;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImACacher
    {
        Task RunHousekeepingSession();
        Task ClearAll();
    }

    public interface ImACacher<T> : ImACacher
    {
        Task<T> GetOrAdd(string id, Func<string, Task<ImCachebale<T>>> cacheableItemFactory);
    }
}
