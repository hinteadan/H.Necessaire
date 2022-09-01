using System;
using System.Linq;

namespace H.Necessaire
{
    public class SecurityContext
    {
        public UserInfo User { get; set; }
        public AuthInfo Auth { get; set; }
        public Role[] Roles { get; set; } = new Role[0];

        public Permission[] EffectivePermissions => FlattenPermissions();

        public bool HasPermission(Guid id, PermissionLevel permissionLevel = PermissionLevel.Locked)
        {
            if (User?.IsIronMan() == true)
                return true;

            if (!Roles?.Any() ?? true)
                return false;

            return Roles.Any(x => x.HasPermission(id, permissionLevel));
        }

        public bool HasPermission(string idTag, PermissionLevel permissionLevel = PermissionLevel.Locked)
        {
            if (User?.IsIronMan() == true)
                return true;

            if (!Roles?.Any() ?? true)
                return false;

            return Roles.Any(x => x.HasPermission(idTag, permissionLevel));
        }

        public bool HasPermission(params PermissionClaim[] permissionClaims)
        {
            if (User?.IsIronMan() == true)
                return true;

            if (!permissionClaims?.Any(x => x != null) ?? true)
                return true;

            return
                permissionClaims
                .Where(x => x != null)
                .All(HasPermissionClaim);
        }

        private bool HasPermissionClaim(PermissionClaim permissionClaim)
        {
            if (permissionClaim == null)
                throw new ArgumentNullException(nameof(permissionClaim), $"{nameof(permissionClaim)} cannot be null");

            if (!Roles?.Any() ?? true)
                return false;

            return Roles.Any(x =>
                (permissionClaim.ID.HasValue ? x.HasPermission(permissionClaim.ID.Value, permissionClaim.MinimumRequiredLevel) : false)
                || x.HasPermission(permissionClaim.IDTag, permissionClaim.MinimumRequiredLevel)
            );
        }

        private Permission[] FlattenPermissions()
        {
            if (!Roles?.Any() ?? true)
                return new Permission[0];

            return
                Roles
                .SelectMany(x => x.Permissions ?? new Permission[0])
                .GroupBy(x => x.ID)
                .Select(x => x.OrderByDescending(p => p.Level).First())
                .ToArray();
        }
    }
}
