using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace H.Necessaire.Runtime.Azure.CosmosDB.Core.Model.SQL
{
    public static class CosmosSqlExtensions
    {
        static readonly string[] rootProps = new string[] { "id", "partitionKey", "dataType", "data" };

        public static string ToCosmosSqlFilterSyntax(this IEnumerable<ImACosmosSqlFilterCriteria> sqlFilters, string binder = "AND", string itemAlias = "doc")
            => sqlFilters?.Any() != true ? null : string.Join($" {binder} ", sqlFilters.ToNoNullsArray()?.Select(x => x.ToString(itemAlias)));

        public static string ToCosmosSqlQuerySyntax(this IEnumerable<ImACosmosSqlFilterCriteria> sqlFilters, CosmosSqlSortCriteria sortCriteria = null, CosmosSqlLimitCriteria limitCriteria = null, string itemAlias = "doc")
        {
            StringBuilder result = new StringBuilder("SELECT * FROM docs ").Append(itemAlias);

            string filterSql = sqlFilters.ToCosmosSqlFilterSyntax(itemAlias: itemAlias);
            if (!filterSql.IsEmpty())
            {
                result
                    .Append(" WHERE ")
                    .Append(filterSql)
                    ;
            }

            if (sortCriteria != null)
            {
                result
                    .Append(" ORDER BY ")
                    .Append(sortCriteria.Property.ToCosmosProperty(itemAlias))
                    .Append(" ")
                    .Append(sortCriteria.SortDirection)
                    ;
            }

            if (limitCriteria != null)
            {
                result
                    .Append(" OFFSET ")
                    .Append(limitCriteria.Offset)
                    .Append(" LIMIT ")
                    .Append(limitCriteria.Count)
                    ;
            }

            return result.ToString();
        }

        public static string ToCosmosSqlCountQuerySyntax(this IEnumerable<ImACosmosSqlFilterCriteria> sqlFilters, CosmosSqlLimitCriteria limitCriteria = null, string itemAlias = "doc")
        {
            StringBuilder result = new StringBuilder("SELECT VALUE COUNT(1) FROM docs ").Append(itemAlias);

            string filterSql = sqlFilters.ToCosmosSqlFilterSyntax(itemAlias: itemAlias);
            if (!filterSql.IsEmpty())
            {
                result
                    .Append(" WHERE ")
                    .Append(filterSql)
                    ;
            }

            if (limitCriteria != null)
            {
                result
                    .Append(" OFFSET ")
                    .Append(limitCriteria.Offset)
                    .Append(" LIMIT ")
                    .Append(limitCriteria.Count)
                    ;
            }

            return result.ToString();
        }

        public static CosmosSqlSortCriteria ToCosmosSqlSortCriteria(this ISortFilter filter)
        {
            if (filter?.SortFilters?.Any() != true)
                return null;

            SortFilter sortFilterToApply = filter.SortFilters.First();

            return
                new CosmosSqlSortCriteria
                {
                    Property = sortFilterToApply.By,
                    SortDirection = sortFilterToApply.Direction == SortFilter.SortDirection.Ascending ? "ASC" : "DESC",
                };
        }

        public static CosmosSqlLimitCriteria ToCosmosSqlLimitCriteria(this IPageFilter filter)
        {
            if (filter?.PageFilter == null)
                return null;

            return
                new CosmosSqlLimitCriteria
                {
                    Offset = filter.PageFilter.PageIndex * filter.PageFilter.PageSize,
                    Count = filter.PageFilter.PageSize,
                };
        }

        public static string ToCosmosProperty(this string property, string itemAlias)
        {
            StringBuilder printer = new StringBuilder();

            if (!itemAlias.IsEmpty())
                printer.Append(itemAlias).Append(".");

            if (property.NotIn(rootProps))
                printer.Append("data").Append(".");

            printer.Append(property);

            return printer.ToString();
        }

        public static IEnumerable<ImACosmosSqlFilterCriteria> ToCosmosSqlFilterCriterias(this object filter, IDictionary<string, object> sqlParams, IDictionary<string, string> filterToStoreMapping = null, string itemAlias = "doc")
        {
            if (filter == null)
                return Enumerable.Empty<ImACosmosSqlFilterCriteria>();

            PropertyInfo[] filterProperties
                = filter
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.Name.NotIn(nameof(ISortFilter.SortFilters), nameof(IPageFilter.PageFilter)))
                .ToNoNullsArray()
                ;

            if (filterProperties?.Any() != true)
                return Enumerable.Empty<ImACosmosSqlFilterCriteria>();

            return
                filterProperties
                .SelectMany(x => filter.AddParamIfNecessary(
                    x.Name,
                    sqlParams,
                    storeProperty: x.Name.Is("IDs") ? "id" : filterToStoreMapping?.ContainsKey(x.Name) != true ? null : filterToStoreMapping[x.Name],
                    @operator:
                        (x.Name.Is("FromInclusive") || x.Name.StartsWith("Min", StringComparison.InvariantCultureIgnoreCase))
                        ? ">="
                        : (x.Name.Is("ToInclusive") || x.Name.StartsWith("Max", StringComparison.InvariantCultureIgnoreCase))
                        ? "<="
                        : x.Name.Is("From")
                        ? ">"
                        : x.Name.Is("To")
                        ? "<"
                        : "="
                ))
                .ToArray()
                ;
        }

        public static IEnumerable<ImACosmosSqlFilterCriteria> AddParamIfNecessary(this object filter, string filterProperty, IDictionary<string, object> sqlParams, string storeProperty = null, string @operator = "=", string itemAlias = "doc")
        {
            if (filter == null)
                return Enumerable.Empty<ImACosmosSqlFilterCriteria>();

            string propertyName = filterProperty;
            string storePropertyName = !storeProperty.IsEmpty() ? storeProperty : ConstructStorePropertyName(propertyName);
            PropertyInfo propertyInfo = filter.GetType().GetProperty(propertyName);
            object propertyValue = propertyInfo.GetValue(filter, null);
            Array propertyValueAsArray = propertyValue as Array;
            if ((propertyValueAsArray?.Length ?? 0) > 0)
            {
                sqlParams.Add(propertyName, propertyValue);
                return new CosmosSqlFilterCriteria(property: storePropertyName, parameterName: propertyName, @operator: "IN").AsArray();
            }
            if (propertyValue != null)
            {
                sqlParams.Add(propertyName, propertyValue);
                return new CosmosSqlFilterCriteria(property: storePropertyName, parameterName: propertyName, @operator: @operator).AsArray();
            }

            return Enumerable.Empty<ImACosmosSqlFilterCriteria>();
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
