using System;
using System.Linq;

namespace H.Necessaire
{
    public class Role : IGuidIdentity
    {
        public Guid ID { get; set; }
        public string IDTag { get; set; }
        public string Name { get; set; }
        public Permission[] Permissions { get; set; } = new Permission[0];

        public bool HasPermission(Guid id, PermissionLevel permissionLevel = PermissionLevel.Locked)
        {
            if (!Permissions?.Any() ?? true)
                return false;

            return
                Permissions.Any(x => x.ID == id && x.Level >= permissionLevel);
        }

        public bool HasPermission(string idTag, PermissionLevel permissionLevel = PermissionLevel.Locked)
        {
            if (!Permissions?.Any() ?? true)
                return false;

            return
                Permissions.Any(x => string.Equals(x.IDTag, idTag, StringComparison.InvariantCultureIgnoreCase) && x.Level >= permissionLevel);
        }
    }
}
