using H.Necessaire;
using System.Collections.Generic;

namespace System.Collections.Concurrent
{
    public class ConcurrentDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public bool TryRemove(TKey key, out TValue val)
        {
            val = default(TValue);
            if (!this.ContainsKey(key))
                return false;
            val = this[key];
            this.Remove(key);
            return true;
        }

        public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            if (this.ContainsKey(key))
                this[key] = updateValueFactory(key, this[key]);
            else
                this.Add(key, addValue);
            return addValue;
        }

        public bool TryAdd(TKey key, TValue addValue)
        {
            bool result = false;

            new Action(() =>
            {
                if (this.ContainsKey(key))
                {
                    result = false;
                    return;
                }
                this.Add(key, addValue);
                result = true;
            })
            .TryOrFailWithGrace(onFail: ex => result = false);


            return result;
        }

        public TValue GetOrAdd(TKey key, TValue value)
        {
            if (!this.ContainsKey(key))
            {
                this.Add(key, value);
                return value;
            }

            return this[key];
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFunc)
        {
            if (!this.ContainsKey(key))
            {
                TValue value = valueFunc(key);
                this.Add(key, value);
                return value;
            }

            return this[key];
        }
    }
}
