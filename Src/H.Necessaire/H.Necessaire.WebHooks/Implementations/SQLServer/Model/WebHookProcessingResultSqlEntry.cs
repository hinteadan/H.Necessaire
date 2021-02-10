using H.Necessaire.Dapper;
using System;

namespace H.Necessaire.WebHooks.Implementations.SQLServer.Model
{
    class WebHookProcessingResultSqlEntry : SqlEntryBase
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public Guid WebHookRequestID { get; set; }
        public DateTime HappenedAt { get; set; } = DateTime.UtcNow;
        public bool IsSuccessful { get; set; }
        public string Reason { get; set; }
        public string CommentsJson { get; set; }
    }
}
