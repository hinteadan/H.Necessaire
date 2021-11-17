using System;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface IKeyValueStorage
    {
        string StoreName { get; }

        Task Set(string key, string value);

        Task SetFor(string key, string value, TimeSpan validFor);

        Task SetUntil(string key, string value, DateTime validUntil);

        Task<string> Get(string key);

        Task Zap(string key);

        Task Remove(string key);
    }
}
