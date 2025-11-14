namespace H.Necessaire
{
    public class Page<T>
    {
        public T[] Content { get; set; } = new T[0];
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 10;
        public int? TotalNumberOfPages { get; set; } = null;
        public long? TotalCount { get; set; } = null;

        public static Page<T> For(IPageFilter pagefilter, long allCount, params T[] content)
        {
            return new Page<T>
            {
                Content = content ?? new T[0],
                PageIndex = pagefilter?.PageFilter?.PageIndex ?? 0,
                PageSize = pagefilter?.PageFilter?.PageSize ?? content?.Length ?? 10,
                TotalNumberOfPages = (pagefilter?.PageFilter?.PageSize ?? 0) > 0
                    ? (int)(allCount / pagefilter.PageFilter.PageSize) + (allCount % pagefilter.PageFilter.PageSize == 0 ? 0 : 1)
                    : 1,
                TotalCount = allCount,
            };
        }

        public static Page<T> Single(params T[] content)
        {
            return new Page<T>
            {
                Content = content ?? new T[0],
                PageIndex = 0,
                PageSize = content?.Length ?? 0,
                TotalNumberOfPages = 1,
                TotalCount = content?.Length ?? 0,
            };
        }

        public static Page<T> Empty(int pageIndex = 0, int pageSize = 0, int totalNumberOfPages = 1)
        {
            return new Page<T>
            {
                Content = new T[0],
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalNumberOfPages = totalNumberOfPages,
                TotalCount = 0,
            };
        }
    }
}
