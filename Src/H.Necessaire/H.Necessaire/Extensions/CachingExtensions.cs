using System;

namespace H.Necessaire
{
    public static class CachingExtensions
    {
        public static ImCachebale<T> ToCacheableItem<T>(this T data, string identifier = null, TimeSpan? cacheDuration = null)
        {
            DateTime now = DateTime.UtcNow;
            return
                new CacheableItem<T>
                {
                    ID = identifier.IsEmpty() ? Guid.NewGuid().ToString() : identifier,
                    Payload = data,
                    CreatedAt = now,
                    ValidFrom = now,
                    AsOf = now,
                }.And(x =>
                {
                    x.ValidFor = cacheDuration;
                });
        }

        public static ImCachebale<T> ToCacheableItem<T>(this T data, string identifier = null, DateTime? expiresAt = null)
        {
            DateTime now = DateTime.UtcNow;
            return
                new CacheableItem<T>
                {
                    ID = identifier.IsEmpty() ? Guid.NewGuid().ToString() : identifier,
                    Payload = data,
                    CreatedAt = now,
                    ValidFrom = now,
                    AsOf = now,
                }.And(x =>
                {
                    x.ExpiresAt = expiresAt;
                });
        }

        public static ImCachebale<T> ToCacheableItem<T>(this T data, string identifier = null) => data.ToCacheableItem(identifier, expiresAt: null);

        public static ImCachebale<T> NoSlidingExpiration<T>(this ImCachebale<T> cacheableItem) => cacheableItem.And(x => x.IsSlidingExpirationDisabled = true);
        public static ImCachebale<T> SlidingExpiration<T>(this ImCachebale<T> cacheableItem) => cacheableItem.And(x => x.IsSlidingExpirationDisabled = false);
    }
}
