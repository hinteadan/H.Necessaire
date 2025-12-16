using System;

namespace H.Necessaire
{
    public class OperationContext : IGuidIdentity
    {
        public static OperationContext Current => CallContext<OperationContext>.GetData(WellKnownCallContextKey.OperationContext);

        public Guid ID { get; set; } = Guid.NewGuid();
        public ConsumerIdentity Consumer { get; set; }
        public UserInfo User { get; set; }
        public Note[] Parameters { get; set; }
        public Note[] Notes { get; set; }
        public DateTime AsOf { get; set; } = DateTime.UtcNow;
    }
}
