using System;
using System.Collections.Generic;

namespace H.Necessaire
{
    public class InMemoryStorageService<TId, TEntity, TFilter>
        : InMemoryStorageServiceBase<TId, TEntity, TFilter>
        where TFilter : IPageFilter, ISortFilter
        where TEntity : IDentityType<TId>
    {
        readonly Func<IEnumerable<TEntity>, TFilter, IEnumerable<TEntity>> filterProcessor;
        public InMemoryStorageService(Func<IEnumerable<TEntity>, TFilter, IEnumerable<TEntity>> filterProcessor)
        {
            this.filterProcessor = filterProcessor;
        }

        protected override IEnumerable<TEntity> ApplyFilter(IEnumerable<TEntity> stream, TFilter filter)
            => filterProcessor?.Invoke(stream, filter);
    }
}
