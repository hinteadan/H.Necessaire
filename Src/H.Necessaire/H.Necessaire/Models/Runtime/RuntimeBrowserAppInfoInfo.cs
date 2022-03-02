using System;

namespace H.Necessaire
{
    public class RuntimeBrowserAppInfoInfo : IStringIdentity
    {
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public string Family { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string ShortName { get; set; }
        public string Engine { get; set; }
        public string EngineVersion { get; set; }
    }
}
