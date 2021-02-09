using System;
using System.Threading.Tasks;

namespace H.Necessaire.WebHooks
{
    public interface IWebHookRequest : IGuidIdentity
    {
        string HandlingHost { get; set; }
        string Source { get; set; }
        DateTime HappenedAt { get; set; }
        Note[] Meta { get; set; }
        Task<T> GetPayload<T>();
    }
}
