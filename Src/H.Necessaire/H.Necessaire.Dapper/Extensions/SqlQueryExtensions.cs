using System.Linq;

namespace H.Necessaire.Dapper
{
    public static class SqlQueryExtensions
    {
        public static SqlSortCriteria[] ToSqlSortCriteria(this ISortFilter filter, bool throwOnValidationError = true)
        {
            if (!filter?.SortFilters?.Any() ?? true)
                return null;

            OperationResult validationResult = filter.ValidateSortFilters();
            if (!validationResult.IsSuccessful)
            {
                if (throwOnValidationError)
                    validationResult.ThrowOnFail();
                return null;
            }

            return
                filter
                .SortFilters
                .Select(x => new SqlSortCriteria
                {
                    ColumnName = x.By,
                    SortDirection = x.Direction == SortFilter.SortDirection.Descending ? "DESC" : "ASC",
                })
                .ToArray()
                ;
        }

        public static SqlLimitCriteria ToSqlLimitCriteria(this IPageFilter filter)
        {
            if (filter?.PageFilter == null)
                return null;

            return
                new SqlLimitCriteria
                {
                    Offset = filter.PageFilter.PageIndex * filter.PageFilter.PageSize,
                    Count = filter.PageFilter.PageSize,
                };
        }
    }
}
