using H.Necessaire.Runtime.Google.FirestoreDB.Core.Model.Querying;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Core
{
    public static class QueryFilterExtensions
    {
        public static IEnumerable<ImAFirestoreCriteria> ToFirestoreFilterCriterias(this object filter, IDictionary<string, Note> filterToStoreMapping = null)
        {
            if (filter == null)
                yield break;

            PropertyInfo[] filterProperties
                = filter
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.Name.NotIn(nameof(ISortFilter.SortFilters), nameof(IPageFilter.PageFilter)))
                .ToNoNullsArray()
                ;

            if (filterProperties?.Any() != true)
                yield break;

            foreach (PropertyInfo property in filterProperties)
            {
                object value = property.GetValue(filter);
                if (value is null)
                    continue;

                yield return BuildCriteriaFor(property, value, filterToStoreMapping);
            }
        }

        public static HsFirestoreSortCriteria ToFirestoreSortCriteria(this ISortFilter filter)
        {
            if (filter?.SortFilters?.Any() != true)
                return null;

            SortFilter sortFilter = filter.SortFilters.First();

            return
                new HsFirestoreSortCriteria
                {
                    Property = sortFilter.By.Parts().Path(),
                    Direction = sortFilter.Direction == SortFilter.SortDirection.Descending ? HsFirestoreSortCriteria.SortDirection.Descending : HsFirestoreSortCriteria.SortDirection.Ascending,
                };
        }

        public static HsFirestoreLimitCriteria ToFirestoreLimitCriteria(this IPageFilter filter)
        {
            if (filter?.PageFilter == null)
                return null;

            return
                new HsFirestoreLimitCriteria
                {
                    Offset = filter.PageFilter.PageIndex * filter.PageFilter.PageSize,
                    Count = filter.PageFilter.PageSize,
                };
        }

        private static ImAFirestoreCriteria BuildCriteriaFor(PropertyInfo filterProperty, object value, IDictionary<string, Note> filterToStoreMapping)
        {
            string storeProperty
                = filterProperty.Name.Is("IDs")
                ? "ID"
                : filterToStoreMapping?.ContainsKey(filterProperty.Name) == true
                ? filterToStoreMapping[filterProperty.Name].Value
                : null
                ;
            storeProperty = !storeProperty.IsEmpty() ? storeProperty : ConstructStorePropertyName(filterProperty.Name);

            string @opertor
                = filterToStoreMapping?.ContainsKey(filterProperty.Name) == true
                ? filterToStoreMapping[filterProperty.Name].ID
                : (filterProperty.Name.Is("FromInclusive") || filterProperty.Name.StartsWith("Min", StringComparison.InvariantCultureIgnoreCase))
                ? ">="
                : (filterProperty.Name.Is("ToInclusive") || filterProperty.Name.StartsWith("Max", StringComparison.InvariantCultureIgnoreCase))
                ? "<="
                : filterProperty.Name.Is("From")
                ? ">"
                : filterProperty.Name.Is("To")
                ? "<"
                : value is IEnumerable
                ? "[=]"
                : value is string
                ? "*="
                : "="
                ;

            return storeProperty.Filter(@opertor, value);
        }

        private static string ConstructStorePropertyName(string propertyName)
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
