using System;

namespace H.Necessaire
{
    public class PermissionClaim
    {
        public Guid? ID { get; set; }
        public string IDTag { get; set; }
        public PermissionLevel MinimumRequiredLevel { get; set; } = PermissionLevel.Locked;
    }
}
