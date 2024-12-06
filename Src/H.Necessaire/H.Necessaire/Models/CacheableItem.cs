using System;

namespace H.Necessaire
{
    public class CacheableItem<T> : EphemeralType<T>, ImCachebale<T>
    {
        public string ID { get; set; } = Guid.NewGuid().ToString();

        public DateTime LastAccessedAt => AsOf;

        public bool IsSlidingExpirationDisabled { get; set; } = false;

        public void PinAccess(DateTime? at = null) => AsOf = at ?? DateTime.UtcNow;
    }
}
