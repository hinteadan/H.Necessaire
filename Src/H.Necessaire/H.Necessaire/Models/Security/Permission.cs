using System;

namespace H.Necessaire
{
    public class Permission : IGuidIdentity
    {
        public Guid ID { get; set; }
        public string IDTag { get; set; }
        public string Name { get; set; }
        public PermissionLevel Level { get; set; } = PermissionLevel.Locked;
    }
}
