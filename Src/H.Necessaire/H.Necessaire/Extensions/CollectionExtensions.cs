﻿using System;
using System.Collections.Generic;
using System.Linq;

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

        public static T[] AsArray<T>(this T value) => new T[] { value };

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

        public static T[] Push<T>(this T[] array, T item)
        {
            if (array == null)
                array = new T[0];

            array = array.Concat(item.AsArray()).ToArray();

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

        public static string Get(this IEnumerable<Note> notes, string id, bool isCaseInsensitive = false)
        {
            return
                notes
                ?.FirstOrDefault(
                    x => string.Equals(id, x.Id, isCaseInsensitive ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture)
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
    }
}
