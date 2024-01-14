using Raven.Client.Documents.Session;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace H.Necessaire.Runtime.RavenDB.Core
{
    public static class RavenDbQueryExtensions
    {
        static readonly ConcurrentDictionary<Type, PropertyInfo[]> filterPropertyInfosCacheDictionary
            = new ConcurrentDictionary<Type, PropertyInfo[]>();
        static readonly ConcurrentDictionary<(Type FilterType, PropertyInfo FilterProperty), (string FilterPropertyName, Note FilterToStoreMapping)> filterToStoreMappingsCacheDictionary
            = new ConcurrentDictionary<(Type FilterType, PropertyInfo FilterProperty), (string FilterPropertyName, Note FilterToStoreMapping)>();

        public static TDocQuery ApplyRavenDbQueryFilter<TFilter, TEntity, TDocQuery>(this TDocQuery querySoFar, TFilter filter, IDictionary<string, Note> filterToStoreMapping = null, string[] filterPropertiesToSkip = null)
            where TDocQuery : IDocumentQueryBase<TEntity, TDocQuery>
        {
            if (querySoFar == null)
                return querySoFar;

            if (filter == null)
                return querySoFar;

            PropertyInfo[] filterProperties = filter.ToFilterPropertyInfos(filterPropertiesToSkip);
            if (filterProperties?.Any() != true)
                return querySoFar;

            TDocQuery result = querySoFar;

            foreach (PropertyInfo filterPropertyInfo in filterProperties)
            {
                object value = filterPropertyInfo.GetValue(filter);
                if (value is null)
                    continue;

                Note? storePropertyMapping = filterPropertyInfo.ToStorePropertyMapping(typeof(TFilter), value, filterToStoreMapping);
                if (storePropertyMapping == null)
                    continue;

                result = result.AppendRvenDbQueryFilter<TEntity, TDocQuery>(value, storePropertyName: storePropertyMapping.Value.Value, @operator: storePropertyMapping.Value.ID);

            }

            return result;
        }

        private static TDocQuery AppendRvenDbQueryFilter<TEntity, TDocQuery>(this TDocQuery query, object value, string storePropertyName, string @operator)
            where TDocQuery : IDocumentQueryBase<TEntity, TDocQuery>
        {
            switch (@operator?.ToUpperInvariant())
            {
                case ">":
                    return query.WhereGreaterThan(storePropertyName, value);
                case ">=":
                    return query.WhereGreaterThanOrEqual(storePropertyName, value);
                case "<":
                    return query.WhereLessThan(storePropertyName, value);
                case "<=":
                    return query.WhereLessThanOrEqual(storePropertyName, value);

                case "IN":
                    return query.WhereIn(storePropertyName, (value as IEnumerable).ToObjectEnumerable());

                case "EXISTS":
                    return query.WhereExists(storePropertyName);

                case "LUCENE":
                    return query.WhereLucene(storePropertyName, value as string);

                case "REGEX":
                    return query.WhereRegex(storePropertyName, value as string);

                case "%LIKE%":
                    return query.WhereRegex(storePropertyName, $"(?i){value}(?-i)");
                case "LIKE%":
                    return query.WhereStartsWith(storePropertyName, value);
                case "%LIKE":
                    return query.WhereEndsWith(storePropertyName, value);

                case "ANY":
                    return query.ContainsAny(storePropertyName, (value as IEnumerable).ToObjectEnumerable());
                case "ALL":
                    return query.ContainsAll(storePropertyName, (value as IEnumerable).ToObjectEnumerable());

                case "<>":
                case "!=":
                    return query.WhereNotEquals(storePropertyName, value);
                case "=":
                case "LIKE":
                default:
                    return query.WhereEquals(storePropertyName, value);
            }
        }

        private static IEnumerable<object> ToObjectEnumerable(this IEnumerable enumerable)
        {
            foreach (object value in enumerable)
            {
                yield return value;
            }
        }

        private static Note? ToStorePropertyMapping(this PropertyInfo filterPropertyInfo, Type filterType, object value, IDictionary<string, Note> filterToStoreMapping = null)
        {
            if (filterPropertyInfo == null)
                return null;
            if (filterType == null)
                return null;

            if (filterToStoreMappingsCacheDictionary.TryGetValue((FilterType: filterType, FilterProperty: filterPropertyInfo), out (string FilterPropertyName, Note FilterToStoreMapping) cachedMapping))
                return cachedMapping.FilterToStoreMapping;

            string storePropertyName
                = filterPropertyInfo.Name.Is("IDs")
                ? "ID"
                : filterToStoreMapping?.ContainsKey(filterPropertyInfo.Name) == true
                ? filterToStoreMapping[filterPropertyInfo.Name].Value
                : null
                ;
            storePropertyName = !storePropertyName.IsEmpty() ? storePropertyName : filterPropertyInfo.Name.ToRavenDbPropertyName();

            string @opertor
                = filterToStoreMapping?.ContainsKey(filterPropertyInfo.Name) == true
                ? filterToStoreMapping[filterPropertyInfo.Name].ID
                : (filterPropertyInfo.Name.Is("FromInclusive") || filterPropertyInfo.Name.StartsWith("Min", StringComparison.InvariantCultureIgnoreCase))
                ? ">="
                : (filterPropertyInfo.Name.Is("ToInclusive") || filterPropertyInfo.Name.StartsWith("Max", StringComparison.InvariantCultureIgnoreCase))
                ? "<="
                : filterPropertyInfo.Name.Is("From")
                ? ">"
                : filterPropertyInfo.Name.Is("To")
                ? "<"
                : value is IEnumerable
                ? "IN"
                : value is string
                ? "="
                : "="
                ;

            Note mapping = storePropertyName.NoteAs(@opertor);

            filterToStoreMappingsCacheDictionary.TryAdd((FilterType: filterType, FilterProperty: filterPropertyInfo), (FilterPropertyName: filterPropertyInfo.Name, FilterToStoreMapping: mapping));

            return mapping;
        }

        private static PropertyInfo[] ToFilterPropertyInfos(this object filter, string[] filterPropertiesToSkip = null)
        {
            if (filter == null)
                return null;

            Type filterType = filter.GetType();

            if (filterPropertyInfosCacheDictionary.TryGetValue(filterType, out PropertyInfo[] cahcedFilterProperties))
                return cahcedFilterProperties;

            PropertyInfo[] filterProperties
                = filter
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.Name.NotIn(nameof(ISortFilter.SortFilters), nameof(IPageFilter.PageFilter)) && (filterPropertiesToSkip?.Any() != true ? true : p.Name.NotIn(filterPropertiesToSkip)))
                .ToNoNullsArray()
                ;

            filterPropertyInfosCacheDictionary.TryAdd(filterType, filterProperties);

            return filterProperties;
        }

        private static string ToRavenDbPropertyName(this string propertyName)
        {
            if (propertyName.EndsWith("statuses", StringComparison.InvariantCultureIgnoreCase))
                return propertyName.Substring(0, propertyName.Length - 2);

            if (propertyName.StartsWith("Min", StringComparison.InvariantCultureIgnoreCase))
                propertyName = propertyName.Substring(3);
            if (propertyName.StartsWith("Max", StringComparison.InvariantCultureIgnoreCase))
                propertyName = propertyName.Substring(3);

            string result = propertyName.Singularize();

            return result;
        }

        private static string Singularize(this string word)
        {
            if (word.IsEmpty())
                return word;

            if (!$"{word.Last()}".Is("s"))
                return word;

            if (word.Length == 1)
                return word;

            return word.Substring(0, word.Length - 1);
        }
    }
}
