using System;

namespace H.Necessaire
{
    public class OperationContext : IGuidIdentity
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public ConsumerIdentity Consumer { get; set; }
        public Note[] Parameters { get; set; }
        public Note[] Notes { get; set; }
        public DateTime AsOf { get; set; } = DateTime.UtcNow;
    }
}
