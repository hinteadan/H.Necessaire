using System;

namespace H.Necessaire
{
    public class RuntimeClientInfo : IStringIdentity
    {
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public string Type { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
    }
}
