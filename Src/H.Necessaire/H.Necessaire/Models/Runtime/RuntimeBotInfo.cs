using System;

namespace H.Necessaire
{
    public class RuntimeBotInfo : IStringIdentity
    {
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Category { get; set; }
        public string Url { get; set; }
        public string OwnerName { get; set; }
        public string OwnerUrl { get; set; }
    }
}
