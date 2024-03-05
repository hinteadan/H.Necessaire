using System;

namespace H.Necessaire
{
    public interface ImCachebale<TPayload> : IEphemeralType<TPayload>, IStringIdentity
    {
        DateTime LastAccessedAt { get; }
        bool IsSlidingExpirationDisabled { get; set; }

        void MarkAccess(DateTime? at = null);
    }
}
