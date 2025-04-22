using System;

namespace H.Necessaire
{
    public class CacheableItem<T> : EphemeralType<T>, ImCachebale<T>
    {
        public string ID { get; set; } = Guid.NewGuid().ToString();

        public DateTime LastAccessedAt => AsOf;

        public bool IsSlidingExpirationDisabled { get; set; } = false;

        public void PinAccess(DateTime? at = null) => AsOf = at?.EnsureUtc() ?? DateTime.UtcNow;

        public override string ToString()
        {
            return $"CachedAs({ID.EllipsizeIfNecessary(maxLength: 50)}): {Payload?.ToString() ?? "NULL"}";
        }

        public static implicit operator CacheableItem<T>(T payload) => payload.ToCacheableItem(identifier: null) as CacheableItem<T>;

        public static implicit operator T(CacheableItem<T> cacheableItem) => cacheableItem?.IsExpired() == false ? cacheableItem.Payload : default(T);
    }
}
