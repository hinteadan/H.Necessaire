using System;

namespace H.Necessaire.Dapper
{
    public class KeyValueSqlEntry : SqlEntryBase
    {
        public Guid ID { get; set; } = Guid.NewGuid();

        public string StoreName { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public long? ExpiresAtTicks { get; set; }

        public bool HasExpired()
        {
            DateTime expiresAt = ExpiresAtTicks == null ? DateTime.MaxValue : new DateTime(ExpiresAtTicks.Value);
            return DateTime.UtcNow > expiresAt;
        }
    }
}
