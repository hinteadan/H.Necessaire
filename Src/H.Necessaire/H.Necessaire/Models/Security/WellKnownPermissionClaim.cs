using System;

namespace H.Necessaire
{
    public static class WellKnownPermissionClaim
    {
        public static readonly PermissionClaim AnalyticsRead = new PermissionClaim
        {
            ID = Guid.Parse("{0060304A-0365-4045-BD2E-0D1BB43C969D}"),
            IDTag = WellKnownPermissionIDTag.Analytics,
            MinimumRequiredLevel = PermissionLevel.Read,
        };
    }
}
