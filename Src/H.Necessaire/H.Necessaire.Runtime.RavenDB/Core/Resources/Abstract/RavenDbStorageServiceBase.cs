using H.Necessaire.RavenDB;
using Raven.Client.Documents.Commands;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.RavenDB
{
    public abstract class RavenDbStorageServiceBase<TId, TEntity, TFilter, TFilterIndex>
        : RavenDbStorageResourceBase<TId, TEntity, TFilter, TFilterIndex>
        , ImAStorageService<TId, TEntity>
        , ImAStorageBrowserService<TEntity, TFilter>
        where TFilter : IPageFilter, ISortFilter
        where TEntity : IDentityType<TId>
        where TFilterIndex : AbstractIndexCreationTask<TEntity>, new()
    {
        #region Construct
        protected override TId GetIdFor(TEntity item) => item.ID;

        private static Lazy<TFilterIndex> filterIndex = new Lazy<TFilterIndex>(() => new TFilterIndex());

        protected override async Task EnsureIndexes()
        {
            await base.EnsureIndexes();
            await EnsureIndex(() => filterIndex.Value);
        }

        protected override IRavenQueryable<TEntity> ApplyFilter(IRavenQueryable<TEntity> query, TFilter filter) => query;

        protected abstract IAsyncDocumentQuery<TEntity> ApplyFilter(IAsyncDocumentQuery<TEntity> query, TFilter filter);

        protected abstract IDocumentQuery<TEntity> ApplyFilterSync(IDocumentQuery<TEntity> query, TFilter filter);

        public override async Task<TEntity[]> Search(TFilter filter)
        {
            await EnsureIndexes();

            using (IAsyncDocumentSession session = NewReadSession())
            {
                IAsyncDocumentQuery<TEntity> documentQuery = session.Advanced.AsyncDocumentQuery<TEntity, TFilterIndex>();

                documentQuery
                    = ApplyFilter(documentQuery, filter)
                    .ApplyRavenDbSortAndPageFilterIfAny(filter)
                    ;

                return await documentQuery.ToArrayAsync();
            }
        }
        #endregion

        async Task<OperationResult> ImAStorageService<TId, TEntity>.Save(TEntity entity)
        {
            OperationResult result = OperationResult.Win();

            await
                new Func<Task>(
                    () => base.Save(entity)
                )
                .TryOrFailWithGrace(
                    onFail: ex => result = OperationResult.Fail(ex)
                );

            return result;
        }

        public async Task<OperationResult<TEntity>> LoadByID(TId id)
        {
            OperationResult<TEntity> result = OperationResult.Win().WithoutPayload<TEntity>();

            await
                new Func<Task>(
                    async () => result = OperationResult.Win().WithPayload(await base.Load(id))
                )
                .TryOrFailWithGrace(
                    onFail: ex => result = OperationResult.Fail(ex).WithoutPayload<TEntity>()
                );

            return result;
        }

        public async Task<OperationResult<TEntity>[]> LoadByIDs(params TId[] ids)
        {
            await EnsureIndexes();

            List<OperationResult<TEntity>> result = new List<OperationResult<TEntity>>(ids.Length);

            using (IAsyncDocumentSession session = NewWriteSession())
            {
                IAsyncDocumentQuery<TEntity> query = session.Advanced.AsyncDocumentQuery<TEntity, TFilterIndex>().WhereIn(x => x.ID, ids);

                StreamQueryStatistics streamQueryStats;
                IAsyncEnumerator<StreamResult<TEntity>> stream = await session.Advanced.StreamAsync(query, out streamQueryStats);

                while (await stream.MoveNextAsync())
                {
                    OperationResult<TEntity> parseResult = OperationResult.Fail().WithoutPayload<TEntity>();

                    new Action(
                        () => parseResult = OperationResult.Win().WithPayload(stream.Current.Document)
                    )
                    .TryOrFailWithGrace(
                        onFail: ex => parseResult = OperationResult.Fail(ex, stream.Current.ToString()).WithoutPayload<TEntity>()
                    );

                    result.Add(parseResult);
                }
            }

            return result.ToArray();
        }

        public async Task<OperationResult> DeleteByID(TId id)
        {
            OperationResult result = OperationResult.Win();

            await
                new Func<Task>(
                    () => base.Delete(id)
                )
                .TryOrFailWithGrace(
                    onFail: ex => result = OperationResult.Fail(ex)
                );

            return result;
        }

        public async Task<OperationResult<TId>[]> DeleteByIDs(params TId[] ids)
        {
            await EnsureIndexes();

            List<OperationResult<TId>> result = new List<OperationResult<TId>>(ids.Length);

            using (IAsyncDocumentSession session = NewWriteSession())
            {
                foreach (TId id in ids)
                {
                    OperationResult<TId> idResult = OperationResult.Win().WithPayload(id);
                    new Action(() => session.Delete(id)).TryOrFailWithGrace(onFail: ex => idResult = OperationResult.Fail(ex).WithPayload(id));
                    result.Add(idResult);
                }

                await session.SaveChangesAsync();
            }

            return result.ToArray();
        }

        public async Task<OperationResult<Page<TEntity>>> LoadPage(TFilter filter)
        {
            OperationResult<Page<TEntity>> result = OperationResult.Win().WithoutPayload<Page<TEntity>>();

            await
                new Func<Task>(
                    async () => result = OperationResult.Win().WithPayload(Page<TEntity>.For(filter, await base.Count(filter), await Search(filter)))
                )
                .TryOrFailWithGrace(
                    onFail: ex => result = OperationResult.Fail(ex).WithoutPayload<Page<TEntity>>()
                );

            return result;
        }

        public async Task<OperationResult<IDisposableEnumerable<TEntity>>> Stream(TFilter filter)
        {
            OperationResult<IDisposableEnumerable<TEntity>> result = OperationResult.Win().WithoutPayload<IDisposableEnumerable<TEntity>>();

            await
                new Func<Task>(
                    async () =>
                    {
                        await EnsureIndexes();

                        IDocumentSession session = NewSyncReadSession();

                        result =
                            OperationResult.Win().WithPayload(
                                new RavenStream<TEntity>(session, ApplyFilterSync(session.Advanced.DocumentQuery<TEntity, TFilterIndex>(), filter).ApplySyncRavenDbSortAndPageFilterIfAny(filter).ToQueryable()) as IDisposableEnumerable<TEntity>
                            );
                    }
                )
                .TryOrFailWithGrace(
                    onFail: ex => result = OperationResult.Fail(ex).WithoutPayload<IDisposableEnumerable<TEntity>>()
                );

            return result;
        }

        public async Task<OperationResult<IDisposableEnumerable<TEntity>>> StreamAll()
        {
            OperationResult<IDisposableEnumerable<TEntity>> result = OperationResult.Win().WithoutPayload<IDisposableEnumerable<TEntity>>();

            await
                new Func<Task>(
                    async () =>
                    {
                        await EnsureIndexes();

                        IDocumentSession session = NewSyncReadSession();

                        result =
                            OperationResult.Win().WithPayload(
                                new RavenStream<TEntity>(session, session.Advanced.DocumentQuery<TEntity, TFilterIndex>().ToQueryable()) as IDisposableEnumerable<TEntity>
                            );
                    }
                )
                .TryOrFailWithGrace(
                    onFail: ex => result = OperationResult.Fail(ex).WithoutPayload<IDisposableEnumerable<TEntity>>()
                );

            return result;
        }
    }
}
