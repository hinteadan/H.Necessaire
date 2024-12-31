using System;

namespace H.Necessaire
{
    public abstract class EphemeralTypeBase : IEphemeralType
    {
        #region Construct
        static readonly TimeSpan? defaultValidity = TimeSpan.FromHours(24);
        TimeSpan? validFor = defaultValidity;
        DateTime? expiresAt = DateTime.UtcNow + defaultValidity;
        DateTime validFrom = DateTime.UtcNow;
        DateTime createdAt = DateTime.UtcNow;
        DateTime asOf = DateTime.UtcNow;
        #endregion

        public DateTime CreatedAt { get => createdAt; set => createdAt = value.EnsureUtc(); }

        public DateTime AsOf { get => asOf; set => asOf = value.EnsureUtc(); }
        public long AsOfTicks { get => AsOf.Ticks; set => AsOf = new DateTime(value, DateTimeKind.Utc); }

        public DateTime ValidFrom
        {
            get => validFrom;
            set
            {
                validFrom = value.EnsureUtc();
                ValidFor = ValidFor;
            }
        }
        public long ValidFromTicks { get => ValidFrom.Ticks; set => ValidFrom = new DateTime(value, DateTimeKind.Utc); }

        public TimeSpan? ValidFor
        {
            get
            {
                return validFor;
            }
            set
            {
                validFor = value;
                if (validFor == null && ExpiresAtTicks != null)
                    ExpiresAtTicks = null;
                else if (validFor != null && ValidFromTicks + ValidForTicks != ExpiresAtTicks)
                    ExpiresAtTicks = ValidFromTicks + ValidForTicks;
            }
        }
        public long? ValidForTicks { get => ValidFor?.Ticks; set => ValidFor = value == null ? null : new TimeSpan?(new TimeSpan(value.Value)); }

        public DateTime? ExpiresAt
        {
            get
            {
                return expiresAt;
            }
            set
            {
                expiresAt = value?.EnsureUtc();
                if (expiresAt == null && ValidFor != null)
                    ValidForTicks = null;
                else if (expiresAt != null && ExpiresAtTicks - ValidFromTicks != ValidForTicks)
                    ValidForTicks = ExpiresAtTicks - ValidFromTicks;
            }
        }
        public long? ExpiresAtTicks { get => ExpiresAt?.Ticks; set => ExpiresAt = value == null ? null : new DateTime?(new DateTime(value.Value, DateTimeKind.Utc)); }


        #region Operations
        public TimeSpan GetAge(DateTime? asOf = null)
        {
            return (asOf?.EnsureUtc() ?? DateTime.UtcNow) - ValidFrom.EnsureUtc();
        }
        public bool IsExpired(DateTime? asOf = null)
        {
            if (ValidFor == null)
                return false;

            return GetAge(asOf).Ticks > ValidFor.Value.Ticks;
        }
        public bool IsActive(DateTime? asOf = null)
        {
            return ValidFrom.Ticks <= (asOf?.EnsureUtc().Ticks ?? DateTime.UtcNow.Ticks) && !IsExpired(asOf);
        }

        public void ActiveAsOf(DateTime asOf) => ValidFrom = asOf.EnsureUtc();
        public void DoNotExpire() => ExpiresAt = null;
        public void ExpireAt(DateTime at) => ExpiresAt = at.EnsureUtc();
        public void ExpireIn(TimeSpan timeSpan) => ValidFor = timeSpan;
        #endregion
    }
}
