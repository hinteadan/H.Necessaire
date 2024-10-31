using H.Necessaire.Runtime.Google.FirestoreDB.Core.Model;
using H.Necessaire.Runtime.Google.FirestoreDB.Core.Model.Querying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Core.Resources.Abstract
{
    public abstract class GoogleFirestoreDbStorageResourceBase<TId, TEntity, TFilter>
        : ImAStorageService<TId, TEntity>
        , ImAStorageBrowserService<TEntity, TFilter>
        , ImADependency
        where TFilter : IPageFilter, ISortFilter
        where TEntity : IDentityType<TId>
    {
        #region Construct
        HsFirestoreStorageService firestoreStorageService;
        static readonly string collectionName = typeof(TEntity).Name;
        public virtual void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            firestoreStorageService = dependencyProvider.Get<HsFirestoreStorageService>()?.WithConfig(_ => _.And(cfg =>
            {
                cfg.CollectionName = collectionName;
            }));
        }
        protected void OverrideConfig(Func<HsFirestoreConfig, HsFirestoreConfig> configDecorator)
        {
            firestoreStorageService = firestoreStorageService?.WithConfig(configDecorator);
        }
        #endregion

        public virtual async Task<OperationResult> Save(TEntity entity)
            => await firestoreStorageService.Save(entity.ToFirestoreDocument(id: entity?.ID?.ToString()), collectionName);

        public virtual async Task<OperationResult> DeleteByID(TId id)
            => await firestoreStorageService.Delete(id?.ToString(), collectionName);

        public virtual async Task<OperationResult<TId>[]> DeleteByIDs(params TId[] ids)
        {
            if (ids?.Any() != true)
                return Array.Empty<OperationResult<TId>>();

            OperationResult result = await firestoreStorageService.DeleteBatch(ids.Select(id => id?.ToString()), collectionName);
            if (!result.IsSuccessful)
                return ids.Select(id => result.WithPayload(id)).ToArray();

            return ids.Select(id => id.ToWinResult()).ToArray();
        }

        public virtual async Task<OperationResult<TEntity>> LoadByID(TId id)
        {
            OperationResult<HsFirestoreDocument> loadResult
                = await firestoreStorageService.Load(id?.ToString(), collectionName);

            if (!loadResult.IsSuccessful)
                return loadResult.WithoutPayload<TEntity>();

            return loadResult.WithPayload(loadResult.Payload.ToData<TEntity>());
        }

        public virtual async Task<OperationResult<TEntity>[]> LoadByIDs(params TId[] ids)
        {
            if (ids?.Any() != true)
                return Array.Empty<OperationResult<TEntity>>();

            OperationResult<ILimitedEnumerable<HsFirestoreDocument>> result =
                await firestoreStorageService.LoadByCustomCriteria(
                    criterias: "ID".Filter("[=]", ids).AsArray(),
                    sortCriteria: null,
                    limitCriteria: null,
                    collectionName: collectionName,
                    @operator: HsFirestoreCompositionOperator.And
                );

            if (!result.IsSuccessful)
                return result.WithoutPayload<TEntity>().AsArray();

            return
                result
                .Payload
                .Select(x =>
                    x.ToData<TEntity>()?.ToWinResult()
                )
                .ToNoNullsArray()
                ;
        }

        public virtual async Task<OperationResult<Page<TEntity>>> LoadPage(TFilter filter)
        {
            OperationResult<ILimitedEnumerable<HsFirestoreDocument>> loadResult =
                await firestoreStorageService.LoadByCustomCriteria(
                    criterias: ApplyFilter(filter),
                    sortCriteria: filter?.ToFirestoreSortCriteria(),
                    limitCriteria: filter?.ToFirestoreLimitCriteria(),
                    collectionName: collectionName,
                    @operator: HsFirestoreCompositionOperator.And
                );

            if (!loadResult.IsSuccessful)
                return loadResult.WithoutPayload<Page<TEntity>>();

            ILimitedEnumerable<HsFirestoreDocument> pageResult = loadResult.Payload;

            return
                Page<TEntity>.For(
                    filter,
                    pageResult.TotalNumberOfItems,
                    pageResult.Select(x => x.ToData<TEntity>()).ToArray()
                )
                .ToWinResult();
        }

        public virtual async Task<OperationResult<IDisposableEnumerable<TEntity>>> Stream(TFilter filter)
        {
            OperationResult<IDisposableEnumerable<HsFirestoreDocument>> loadResult =
                await firestoreStorageService.StreamByCustomCriteria(
                    criterias: ApplyFilter(filter),
                    sortCriteria: filter?.ToFirestoreSortCriteria(),
                    limitCriteria: filter?.ToFirestoreLimitCriteria(),
                    collectionName: collectionName,
                    @operator: HsFirestoreCompositionOperator.And
                );

            if (!loadResult.IsSuccessful)
                return loadResult.WithoutPayload<IDisposableEnumerable<TEntity>>();

            return
                loadResult
                .Payload
                .ProjectTo(x => x.ToData<TEntity>())
                .ToWinResult()
                ;
        }

        public virtual async Task<OperationResult<IDisposableEnumerable<TEntity>>> StreamAll()
        {
            OperationResult<IDisposableEnumerable<HsFirestoreDocument>> loadResult = await firestoreStorageService.StreamAll(collectionName);

            if (!loadResult.IsSuccessful)
                return loadResult.WithoutPayload<IDisposableEnumerable<TEntity>>();

            return
                loadResult
                .Payload
                .ProjectTo(x => x.ToData<TEntity>())
                .ToWinResult()
                ;
        }



        protected virtual ImAFirestoreCriteria[] ApplyFilter(TFilter filter)
        {
            return
                filter
                ?.ToFirestoreFilterCriterias(FilterToStoreMapping)
                .ToArrayNullIfEmpty()
                ;
        }
        protected virtual IDictionary<string, Note> FilterToStoreMapping => null;
    }
}
