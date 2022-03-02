using System;

namespace H.Necessaire
{
    public class RuntimeTrace : IGuidIdentity
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public Guid? ConsumerIdentityID { get; set; }
        public DateTime AsOf { get; set; } = DateTime.UtcNow;
        public InternalIdentity RuntimeTraceProvider { get; set; }

        public RuntimeDeviceInfo Device { get; set; }
        public RuntimeOperatingSystemInfo OperatingSystem { get; set; }
        public RuntimeClientInfo Client { get; set; }
        public RuntimeBrowserAppInfoInfo Browser { get; set; }
        public RuntimeBotInfo Bot { get; set; }
        public RuntimeTimingInfo Timing { get; set; }

        public Note[] Notes { get; set; }
    }
}
