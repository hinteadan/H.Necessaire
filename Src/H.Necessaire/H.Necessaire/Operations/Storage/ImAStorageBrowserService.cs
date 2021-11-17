using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImAStorageBrowserService<TEntity, TFilter> where TFilter : IPageFilter, ISortFilter
    {
        Task<OperationResult<Page<TEntity>>> LoadPage(TFilter filter);

        Task<OperationResult<IDisposableEnumerable<TEntity>>> Stream(TFilter filter);

        Task<OperationResult<IDisposableEnumerable<TEntity>>> StreamAll();
    }
}
