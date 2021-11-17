using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire.Operations.Storage.Concrete
{
    internal class InMemoryTableStore<TKey, TItem>
    {
        private readonly ConcurrentDictionary<TKey, TItem> dataDictionary = new ConcurrentDictionary<TKey, TItem>();

        public void Save(TKey id, TItem item)
        {
            dataDictionary.AddOrUpdate(id, item, (x, y) => item);
        }

        public TItem Load(TKey id)
        {
            if (!dataDictionary.ContainsKey(id))
                return default(TItem);

            return dataDictionary[id];
        }

        public void Delete(TKey id)
        {
            if (!dataDictionary.ContainsKey(id))
            {
                return;
            }

            TItem deletedItem;
            dataDictionary.TryRemove(id, out deletedItem);
        }

        public long DeleteBulk(Predicate<TItem> predicate)
        {
            long cnt = 0;
            foreach (KeyValuePair<TKey, TItem> entry in FilterData(predicate))
            {
                cnt++;
                Delete(entry.Key);
            }
            return cnt;
        }

        public TItem[] Filter(Predicate<TItem> predicate)
        {
            return FilterData(predicate)
                .Select(x => x.Value)
                .ToArray();
        }

        public long Count(Predicate<TItem> predicate = null)
        {
            return FilterData(predicate).LongCount();
        }

        private KeyValuePair<TKey, TItem>[] FilterData(Predicate<TItem> predicate)
        {
            return dataDictionary
                .Where(x => predicate?.Invoke(x.Value) ?? true)
                .ToArray();
        }
    }
}
