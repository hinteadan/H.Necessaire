using System;
using System.Threading.Tasks;

namespace H.Necessaire.WebHooks
{
    public abstract class WebHookRequestBase : IWebHookRequest
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public string HandlingHost { get; set; }
        public string Source { get; set; }
        public DateTime HappenedAt { get; set; } = DateTime.UtcNow;
        public Note[] Meta { get; set; } = new Note[0];

        public abstract Task<T> GetPayload<T>();
    }
}
