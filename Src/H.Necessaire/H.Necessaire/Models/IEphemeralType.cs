using System;

namespace H.Necessaire
{
    public interface IEphemeralType
    {
        DateTime CreatedAt { get; }
        DateTime AsOf { get; }
        DateTime ValidFrom { get; }
        TimeSpan? ValidFor { get; }
        DateTime? ExpiresAt { get; }
        TimeSpan GetAge(DateTime? asOf = null);
        bool IsExpired(DateTime? asOf = null);
        bool IsActive(DateTime? asOf = null);
        void ActiveAsOf(DateTime asOf);
        void DoNotExpire();
        void ExpireAt(DateTime at);
        void ExpireIn(TimeSpan timeSpan);
    }

    public interface IEphemeralType<TPayload>
    {
        TPayload Payload { get; }
    }
}
