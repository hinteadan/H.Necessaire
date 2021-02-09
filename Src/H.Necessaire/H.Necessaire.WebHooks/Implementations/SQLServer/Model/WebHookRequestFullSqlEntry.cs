using H.Necessaire.Dapper;
using System;

namespace H.Necessaire.WebHooks.Implementations.SQLServer.Model
{
    class WebHookRequestFullSqlEntry : SqlEntryBase
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public string HandlingHost { get; set; }
        public string Source { get; set; }
        public DateTime HappenedAt { get; set; } = DateTime.UtcNow;
        public string MetaJson { get; set; }
        public string PayloadJson { get; set; }
    }
}
