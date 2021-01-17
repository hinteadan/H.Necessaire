using System;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface IKeyValueStorage
    {
        string StoreName { get; }

        Task Set(string key, string value, TimeSpan? validFor = null);

        Task Set(string key, string value, DateTime? validUntil = null);

        Task<string> Get(string key);

        Task Zap(string key);

        Task Remove(string key);
    }
}
