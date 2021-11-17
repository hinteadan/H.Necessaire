using Raven.Client.Documents.Session;
using System.Linq;

namespace H.Necessaire.Runtime.RavenDB
{
    public static class RavenQueryExtensions
    {
        public static IAsyncDocumentQuery<TEntity> ApplyRavenDbSortAndPageFilterIfAny<TEntity, TFilter>(this IAsyncDocumentQuery<TEntity> ravenQueryable, TFilter filter, bool throwOnValidationError = true)
            where TFilter : ISortFilter, IPageFilter
        {
            if (filter?.SortFilters?.Any() ?? false)
            {
                OperationResult validation = filter.ValidateSortFilters();
                if (!validation.IsSuccessful)
                {
                    if (throwOnValidationError) validation.ThrowOnFail();
                    return ravenQueryable;
                }

                ravenQueryable
                    = filter.SortFilters.First().Direction == SortFilter.SortDirection.Ascending
                    ? ravenQueryable.OrderBy(filter.SortFilters.First().By, OrderingType.AlphaNumeric)
                    : ravenQueryable.OrderByDescending(filter.SortFilters.First().By, OrderingType.AlphaNumeric)
                    ;

                foreach (SortFilter sortFilter in filter.SortFilters.Jump(1))
                {
                    ravenQueryable.AddOrder(sortFilter.By, descending: sortFilter.Direction == SortFilter.SortDirection.Descending, OrderingType.AlphaNumeric);
                }
            }

            if (filter?.PageFilter != null)
            {
                ravenQueryable
                    = ravenQueryable
                    .Skip(filter.PageFilter.PageIndex * filter.PageFilter.PageSize)
                    .Take(filter.PageFilter.PageSize)
                    ;
            }

            return ravenQueryable;
        }
    }
}
