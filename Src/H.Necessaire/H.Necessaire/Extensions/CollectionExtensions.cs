using H.Necessaire.Operations.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace H.Necessaire
{
    public static class CollectionExtensions
    {
        public static bool In<T>(this T item, params T[] collection)
        {
            return item.In(collection.AsEnumerable());
        }
        public static bool NotIn<T>(this T item, params T[] collection) => !In(item, collection);

        public static bool In<T>(this T item, IEnumerable<T> collection)
        {
            return item.In(collection, (x, y) => x?.Equals(y) ?? x == null);
        }
        public static bool NotIn<T>(this T item, IEnumerable<T> collection) => !In(item, collection);

        public static bool In<T>(this T item, IEnumerable<T> collection, Func<T, T, bool> comparer)
        {
            return collection?.Any(x => comparer.Invoke(item, x)) ?? false;
        }
        public static bool NotIn<T>(this T item, IEnumerable<T> collection, Func<T, T, bool> comparer) => !In(item, collection, comparer);

        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> collection, int batchSize)
        {
            if (collection == null)
                return null;

            if (!collection.Any())
                return Enumerable.Empty<IEnumerable<T>>();

            if (batchSize < 1)
                throw new InvalidOperationException("The batch size must be higher than zero");

            if (batchSize >= collection.Count())
                return new IEnumerable<T>[] { collection };

            List<IEnumerable<T>> result = new List<IEnumerable<T>>();

            List<T> batch = new List<T>();
            foreach (T item in collection)
            {
                if (batch.Count == batchSize)
                {
                    result.Add(batch.ToArray());
                    batch.Clear();
                }

                batch.Add(item);
            }

            if (batch.Any())
                result.Add(batch.ToArray());

            return result.ToArray();
        }

        public static IEnumerable<T> ApplySortFilterIfAny<T>(this IEnumerable<T> collection, ISortFilter filter, bool throwOnValidationError = true)
        {
            IEnumerable<T> result = collection;

            if (!filter?.SortFilters?.Any() ?? true)
                return result;

            OperationResult validation = filter.ValidateSortFilters();
            if (!validation.IsSuccessful)
            {
                if (throwOnValidationError) validation.ThrowOnFail();
                return result;
            }

            IOrderedEnumerable<T> orderedResult = result.OrderBy(x => 1);
            foreach (SortFilter sortFilter in filter.SortFilters)
            {
                PropertyInfo sortProperty = typeof(T).GetProperty(sortFilter.By);
                switch (sortFilter.Direction)
                {
                    case SortFilter.SortDirection.Descending:
                        orderedResult = orderedResult.ThenByDescending(x => sortProperty.GetValue(x));
                        break;
                    case SortFilter.SortDirection.Ascending:
                    default:
                        orderedResult = orderedResult.ThenBy(x => sortProperty.GetValue(x));
                        break;
                }
            }
            result = orderedResult;

            return result;
        }

        public static IEnumerable<T> ApplyPageFilterIfAny<T>(this IEnumerable<T> collection, IPageFilter filter)
        {
            IEnumerable<T> result = collection;

            if (filter?.PageFilter == null)
                return result;

            result
                = collection
                .Skip(filter.PageFilter.PageIndex * filter.PageFilter.PageSize)
                .Take(filter.PageFilter.PageSize)
                ;

            return result;
        }

        public static IEnumerable<T> ApplySortAndPageFilterIfAny<T, TFilter>(this IEnumerable<T> collection, TFilter filter, bool throwOnValidationError = true)
            where TFilter : ISortFilter, IPageFilter
        {
            return
                collection
                .ApplySortFilterIfAny(filter, throwOnValidationError)
                .ApplyPageFilterIfAny(filter)
                ;
        }

        public static IDisposableEnumerable<T> ToDisposableEnumerable<T>(this IEnumerable<T> collection) => new DataStream<T>(collection);

        public static T[] AsArray<T>(this T value) => new T[] { value };

        public static string[] ToStringArray<T>(this T[] array) => array?.Select(x => x?.ToString()).ToArray();

        public static T[] Jump<T>(this T[] array, int numberOfElementsToJump)
        {
            if (array == null)
                return null;

            if (numberOfElementsToJump < 1)
                return array;

            if (numberOfElementsToJump >= array.Length)
                return new T[0];

            T[] result = new T[array.Length - numberOfElementsToJump];

            for (var i = numberOfElementsToJump; i < array.Length; i++)
            {
                result[i - numberOfElementsToJump] = array[i];
            }

            return result;
        }

        public static T[] Push<T>(this T[] array, T item, bool checkDistinct = true)
        {
            if (array == null)
                array = new T[0];

            array = checkDistinct ? array.Union(item.AsArray()).ToArray() : array.Concat(item.AsArray()).ToArray();

            return array;
        }

        public static T[] Set<T>(this T[] array, Predicate<T> predicate, T item)
        {
            if (array == null)
                array = new T[0];

            int existingIndex = Array.FindIndex(array, predicate ?? (x => false));
            if (existingIndex < 0)
                array = array.Push(item);
            else
                array[existingIndex] = item;

            return array;
        }

        public static T[] Remove<T>(this T[] array, Predicate<T> predicate)
        {
            if (predicate == null || (!array?.Any() ?? true))
                return array;

            array = array.Where(x => !predicate(x)).ToArray();

            return array;
        }

        public static T Get<T>(this T[] array, Predicate<T> predicate, T defaultTo = default(T))
        {
            if (predicate == null || (!array?.Any() ?? true))
                return defaultTo;

            int existingIndex = Array.FindIndex(array, predicate);
            if (existingIndex < 0)
                return defaultTo;

            return array[existingIndex];
        }

        public static string Get(this IEnumerable<Note> notes, string id, bool ignoreCase = false)
        {
            return
                notes
                ?.FirstOrDefault(
                    x => string.Equals(id, x.ID, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture)
                )
                .Value;
        }

        public static string[] TrimToValidKeywordsOnly(this string[] keywords, int minLength = 3, int maxNumberOfKeywords = 3)
        {
            if (keywords == null)
                return keywords;

            if (!keywords.Any())
                return keywords;

            return keywords
                .Where(k =>
                    !string.IsNullOrWhiteSpace(k)
                    && k.Length >= minLength
                )
                .Take(maxNumberOfKeywords)
                .ToArray();
        }

        public static ImAnAuditEntry LatestOrDefault(this IEnumerable<ImAnAuditEntry> auditEntries)
        {
            if (!auditEntries?.Any() ?? true)
                return null;

            return
                auditEntries
                .Where(x => x != null)
                .OrderByDescending(x => x.HappenedAt)
                .FirstOrDefault();
        }
    }
}
