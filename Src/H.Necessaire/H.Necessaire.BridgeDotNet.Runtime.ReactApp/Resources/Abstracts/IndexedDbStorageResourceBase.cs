using System;
using System.Linq;
using System.Threading.Tasks;
using static Retyped.dexie;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp.Resources.Abstracts
{
    public abstract class IndexedDbStorageResourceBase<TIndexedDbStorage, TId, TEntity, TFilter>
        : IndexedDbResourceBase<TIndexedDbStorage, TEntity, TId>
        , ImAStorageService<TId, TEntity>
        , ImAStorageBrowserService<TEntity, TFilter>
        where TIndexedDbStorage : ImAnIndexedDBStorage
        where TEntity : IDentityType<TId>
        where TFilter : IPageFilter, ISortFilter
    {
        #region Construct
        Dexie.Table<object, object> table = null;
        Dexie.Table<object, object> Table { get { if (table == null) table = storage.GetTable(TableName); return table; } }
        protected abstract string TableName { get; }

        protected abstract Dexie.Collection<object, object> ApplyFilter(Dexie.Collection<object, object> collection, TFilter filter);
        #endregion

        public virtual async Task<OperationResult> DeleteByID(TId id)
        {
            OperationResult result = OperationResult.Fail();

            await
                new Func<Task>(async () =>
                {
                    await base.Delete(Table, id);
                })
                .TryOrFailWithGrace(onFail: ex => result = OperationResult.Fail(ex));

            return result;
        }

        public virtual async Task<OperationResult<TId>[]> DeleteByIDs(params TId[] ids)
        {
            OperationResult<TId>[] result = new OperationResult<TId>[0];

            await
                new Func<Task>(async () =>
                {
                    result = await
                        Task.WhenAll(
                            ids.Select(
                                id => DeleteByID(id).ContinueWith(x => x.Result.WithPayload(id))
                            ).ToArray()
                        );
                })
                .TryOrFailWithGrace(onFail: ex => result = ids.Select(id => OperationResult.Fail(ex).WithPayload(id)).ToArray());

            return result;
        }

        public virtual async Task<OperationResult<TEntity>> LoadByID(TId id)
        {
            OperationResult<TEntity> result = OperationResult.Fail().WithoutPayload<TEntity>();

            await
                new Func<Task>(async () =>
                {
                    result
                        = OperationResult.Win().WithPayload(await base.Load(Table, id));
                })
                .TryOrFailWithGrace(onFail: ex => result = OperationResult.Fail(ex).WithoutPayload<TEntity>());

            return result;
        }

        public virtual async Task<OperationResult<TEntity>[]> LoadByIDs(params TId[] ids)
        {
            OperationResult<TEntity>[] result = new OperationResult<TEntity>[0];

            await
                new Func<Task>(async () =>
                {
                    result = await
                        Task.WhenAll(
                            ids.Select(
                                id => LoadByID(id)
                            ).ToArray()
                        );
                })
                .TryOrFailWithGrace(onFail: ex => result = ids.Select(id => OperationResult.Fail(ex).WithoutPayload<TEntity>()).ToArray());

            return result;
        }

        public virtual async Task<OperationResult> Save(TEntity entity)
        {
            OperationResult result = OperationResult.Fail();

            await
                new Func<Task>(async () =>
                {
                    await base.Save(Table, entity);
                })
                .TryOrFailWithGrace(onFail: ex => result = OperationResult.Fail(ex));

            return result;
        }

        public virtual async Task<OperationResult<Page<TEntity>>> LoadPage(TFilter filter)
        {
            OperationResult<IDisposableEnumerable<TEntity>> filterResult = await Stream(filter);

            if (!filterResult.IsSuccessful)
                return filterResult.WithPayload(Page<TEntity>.Empty());

            TaskCompletionSource<long> countLoader = new TaskCompletionSource<long>();
            Table.count().then(x => { countLoader.SetResult((long)x); return x; });
            long allCount = await countLoader.Task;

            using (filterResult.Payload)
            {
                return
                    OperationResult.Win().WithPayload(Page<TEntity>.For(filter, allCount, filterResult.Payload.ToArray()));
            }
        }

        public virtual async Task<OperationResult<IDisposableEnumerable<TEntity>>> Stream(TFilter filter)
        {
            Dexie.Collection<object, object> result = ApplyDexieFilterChain(filter);

            TaskCompletionSource<object[]> dataLoader = new TaskCompletionSource<object[]>();
            result.toArray().then(x => { dataLoader.SetResult(x.ToArray()); return x; });
            object[] data = await dataLoader.Task;
            return OperationResult.Win().WithPayload(data.Select(x => ProjectDexieObject(x)).ToDisposableEnumerable());
        }

        public virtual async Task<OperationResult<IDisposableEnumerable<TEntity>>> StreamAll()
        {
            TaskCompletionSource<object[]> dataLoader = new TaskCompletionSource<object[]>();
            Table.toArray().then(x => { dataLoader.SetResult(x.ToArray()); return x; });
            object[] data = await dataLoader.Task;
            return OperationResult.Win().WithPayload(data.Select(x => ProjectDexieObject(x)).ToDisposableEnumerable());
        }

        protected Dexie.Collection<object, object> ApplyPageFilter(Dexie.Collection<object, object> collection, IPageFilter filter)
        {
            if (filter?.PageFilter == null)
                return collection;

            return
                collection
                .offset(filter.PageFilter.PageIndex * filter.PageFilter.PageSize)
                .limit(filter.PageFilter.PageSize)
                ;
        }

        protected Dexie.Collection<object, object> ApplySortFilter(ISortFilter sortFilter, bool throwOnValidationError = true)
        {
            OperationResult validationResult = sortFilter.ValidateSortFilters();
            if (!validationResult.IsSuccessful)
            {
                if (throwOnValidationError)
                    validationResult.ThrowOnFail();

                return Table.toCollection();
            }

            if (!sortFilter?.SortFilters?.Any() ?? true)
                return Table.toCollection();

            bool hasMultipleSorters = sortFilter.SortFilters.Length > 1;
            if (hasMultipleSorters && throwOnValidationError)
            {
                throw new NotSupportedException("IndexedDB Limitations prevent sorting by multiple properties. You can sort only by a single criteria.");
            }
            SortFilter sort = sortFilter.SortFilters.First();

            Dexie.Collection<object, object> result = Table.orderBy(sort.By);

            if (sort.Direction == SortFilter.SortDirection.Descending)
                result = result.reverse();

            return result;
        }

        protected override Dexie.Collection<object, object> ApplyFilter(object filter) => ApplyDexieFilterChain(filter.As<TFilter>());

        protected override TId GetIdFor(TEntity item) => item.ID;

        private Dexie.Collection<object, object> ApplyDexieFilterChain(TFilter filter)
        {
            return ApplyPageFilter(ApplyFilter(ApplySortFilter(filter), filter), filter);
        }
    }
}
