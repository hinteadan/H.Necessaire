using System;

namespace H.Necessaire
{
    public class RuntimeOperatingSystemInfo : IStringIdentity
    {
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public string Family { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Platform { get; set; }
        public string Version { get; set; }

        public bool? IsWindows { get; set; }
        public string WindowsVersion { get; set; }
    }
}
