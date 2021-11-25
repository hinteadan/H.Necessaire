using System;

namespace H.Necessaire
{
    public class QdAction : IGuidIdentity
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public DateTime QdAt { get; set; } = DateTime.UtcNow;
        public string Type { get; set; }
        public string Payload { get; set; }
        public QdActionStatus Status { get; set; } = QdActionStatus.Queued;
        public int RunCount { get; set; } = 0;

        public override string ToString()
        {
            return $"{Type}({ID} - Attempt: {RunCount + 1})";
        }

        public static QdAction New(string type, string payload)
        {
            return
                new QdAction
                {
                    Type = type,
                    Payload = payload,
                };
        }
    }
}
