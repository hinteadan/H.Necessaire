using System;

namespace H.Necessaire
{
    public static class CachingExtensions
    {
        public static ImCachebale<T> ToCacheableItem<T>(this T data, string identifier, TimeSpan? cacheDuration)
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

        public static ImCachebale<T> ToCacheableItem<T>(this T data, string identifier, DateTime? expiresAt)
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

        public static ImCachebale<T> ToCacheableItem<T>(this T data, TimeSpan? cacheDuration) => data.ToCacheableItem(identifier: null, cacheDuration);
        public static ImCachebale<T> ToCacheableItem<T>(this T data, DateTime? expiresAt) => data.ToCacheableItem(identifier: null, expiresAt);

        public static ImCachebale<T> DontSlideExpiration<T>(this ImCachebale<T> cacheableItem)
        {
            if (cacheableItem == null)
                return cacheableItem;

            cacheableItem.IsSlidingExpirationDisabled = true;

            return cacheableItem;
        }


        /// <summary>
        /// Alias for DontSlideExpiration
        /// </summary>
        public static ImCachebale<T> AbsoluteExpiration<T>(this ImCachebale<T> cacheableItem)
            => cacheableItem.DontSlideExpiration();

        /// <summary>
        /// Alias for DontSlideExpiration
        /// </summary>
        public static ImCachebale<T> FixedExpiration<T>(this ImCachebale<T> cacheableItem)
            => cacheableItem.DontSlideExpiration();

        /// <summary>
        /// Alias for DontSlideExpiration
        /// </summary>
        public static ImCachebale<T> NoSlidingExpiration<T>(this ImCachebale<T> cacheableItem)
            => cacheableItem.DontSlideExpiration();


        /// <summary>
        /// Default behavior, no need to explicitly call this, unless your logic can potentially disable sliding expiration and you want to make sure it's enabled
        /// </summary>
        /// <typeparam name="TCacheable">ImCachebale<T></typeparam>
        /// <typeparam name="T">cached data type</typeparam>
        /// <param name="cacheableItem">concrete cacheable item, use ToCacheableItem() methods</param>
        /// <returns>Returns back the provided cacheableItem, for fluent syntax</returns>
        public static ImCachebale<T> SlideExpiration<T>(this ImCachebale<T> cacheableItem)
        {
            if (cacheableItem == null)
                return cacheableItem;

            cacheableItem.IsSlidingExpirationDisabled = false;

            return cacheableItem;
        }

        /// <summary>
        /// Alias for SlideExpiration
        /// </summary>
        public static ImCachebale<T> SlidingExpiration<T>(this ImCachebale<T> cacheableItem)
            => cacheableItem.SlideExpiration();
    }
}
