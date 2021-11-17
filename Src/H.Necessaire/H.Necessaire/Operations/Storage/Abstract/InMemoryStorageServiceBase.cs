using H.Necessaire.Operations.Concrete;
using H.Necessaire.Operations.Storage.Concrete;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public abstract class InMemoryStorageServiceBase<TId, TEntity, TFilter>
        : ImAStorageService<TId, TEntity>
        , ImAStorageBrowserService<TEntity, TFilter>
        where TFilter : IPageFilter, ISortFilter
        where TEntity : IDentityType<TId>
    {
        #region Construct
        readonly InMemoryTableStore<TId, TEntity> inMemoryTableStore = new InMemoryTableStore<TId, TEntity>();
        #endregion

        public Task<OperationResult> Save(TEntity entity)
        {
            inMemoryTableStore.Save(entity.ID, entity);
            return OperationResult.Win().AsTask();
        }

        public Task<OperationResult<TEntity>> LoadByID(TId id)
        {
            return OperationResult.Win().WithPayload(inMemoryTableStore.Load(id)).AsTask();
        }

        public Task<OperationResult<TEntity>[]> LoadByIDs(params TId[] ids)
        {
            return
                inMemoryTableStore.Filter(x => x.ID.In(ids))
                .Select(x => OperationResult.Win().WithPayload(x))
                .ToArray()
                .AsTask()
                ;
        }

        public Task<OperationResult> DeleteByID(TId id)
        {
            inMemoryTableStore.Delete(id);
            return OperationResult.Win().AsTask();
        }

        public Task<OperationResult<TId>[]> DeleteByIDs(params TId[] ids)
        {
            inMemoryTableStore.DeleteBulk(x => x.ID.In(ids));
            return ids.Select(x => OperationResult.Win().WithPayload(x)).ToArray().AsTask();
        }

        public Task<OperationResult<IDisposableEnumerable<TEntity>>> StreamAll()
        {
            IDisposableEnumerable<TEntity> payload = new DataStream<TEntity>(inMemoryTableStore.Filter(x => true));
            return OperationResult.Win().WithPayload(payload).AsTask();
        }

        public async Task<OperationResult<IDisposableEnumerable<TEntity>>> Stream(TFilter filter)
        {
            OperationResult<IDisposableEnumerable<TEntity>> allStream = await StreamAll();
            using (IDisposableEnumerable<TEntity> all = allStream.Payload)
            {
                IDisposableEnumerable<TEntity> payload = new DataStream<TEntity>(ApplyFilter(all, filter));
                return OperationResult.Win().WithPayload(payload);
            }
        }

        public async Task<OperationResult<Page<TEntity>>> LoadPage(TFilter filter)
        {
            OperationResult<IDisposableEnumerable<TEntity>> filteredStream = await Stream(filter);
            using (IDisposableEnumerable<TEntity> filtered = filteredStream.Payload)
            {
                return OperationResult.Win().WithPayload(Page<TEntity>.For(filter, inMemoryTableStore.Count(x => true), filtered.ToArray()));
            }
        }

        protected abstract IEnumerable<TEntity> ApplyFilter(IEnumerable<TEntity> stream, TFilter filter);
    }
}
