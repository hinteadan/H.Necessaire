using H.Necessaire.BridgeDotNet.Runtime.ReactApp.Core.Model.AppState.Abstract;
using System;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp.Core.Model.AppState
{
    public class SecurityContextAppStateEntry : AppStateEntryBase<SecurityContext>
    {
        public SecurityContextAppStateEntry() : base()
        {
            ExpireIn(TimeSpan.FromMinutes(55));
        }

        public const string DefaultID = nameof(SecurityContextAppStateEntry);
        public override string ID => DefaultID;

        public SecurityContext SecurityContext => Payload;

        public static SecurityContextAppStateEntry From(SecurityContext securityContext)
        {
            return
                new SecurityContextAppStateEntry
                {
                    Payload = securityContext,
                }
                .And(x =>
                {
                    if (securityContext?.Auth != null)
                    {
                        DateTime validFrom = securityContext.Auth.AccessTokenGeneratedAt.EnsureUtc();
                        if (validFrom > DateTime.UtcNow)
                            validFrom = DateTime.UtcNow;

                        DateTime expiresAt = securityContext.Auth.AccessTokenExpiresAt.EnsureUtc().AddMinutes(-5);
                        if (expiresAt <= validFrom)
                            expiresAt = validFrom.AddMinutes(55);

                        x.ActiveAsOf(validFrom);
                        x.ExpireAt(expiresAt);
                    }
                });
        }
    }
}
