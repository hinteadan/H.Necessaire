using H.Necessaire.Runtime.Azure.CosmosDB.Core.Model.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Azure.CosmosDB.Core.Resources.Abstract
{
    public abstract class AzureCosmosDbStorageResourceBase<TId, TEntity, TFilter>
        : ImAStorageService<TId, TEntity>
        , ImAStorageBrowserService<TEntity, TFilter>
        , ImADependency
        where TFilter : IPageFilter, ISortFilter
        where TEntity : IDentityType<TId>
    {
        #region Construct
        HsCosmosStorageService cosmosStorageService;
        public virtual void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            cosmosStorageService = dependencyProvider.Get<HsCosmosStorageService>();
        }

        protected void OverrideConfig(Func<HsCosmosConfig, HsCosmosConfig> configDecorator)
        {
            cosmosStorageService = cosmosStorageService?.WithConfig(configDecorator);
        }
        #endregion

        public virtual async Task<OperationResult> Save(TEntity entity)
            => await cosmosStorageService.Save(entity.ToCosmosItem());

        public virtual async Task<OperationResult<TEntity>> LoadByID(TId id)
            => await cosmosStorageService.Load<TEntity>(id.ToString());

        public virtual async Task<OperationResult<TEntity>[]> LoadByIDs(params TId[] ids)
        {
            if (ids?.Any() != true)
                return new OperationResult<TEntity>[0];

            OperationResult<IDisposableEnumerable<TEntity>> streamResult
                = await cosmosStorageService.StreamByCustomCriteria<TEntity>(
                    sqlFilters: new CosmosSqlFilterCriteria("id", "ids", "IN").AsArray(),
                    sqlParams: new Dictionary<string, object>().And(x =>
                    {
                        x.Add("ids", ids);
                    }),
                    partitionKey: typeof(TEntity).ToPartitionKey()
                );

            if (!streamResult.IsSuccessful)
                return streamResult.WithoutPayload<TEntity>().AsArray();

            TEntity[] loadedEntites;
            using (IDisposableEnumerable<TEntity> stream = streamResult.Payload)
            {
                loadedEntites = stream.ToArray();
            }

            TId[] loadedIDs = loadedEntites?.Select(x => x.ID).ToArray();
            TId[] notLoadedIDs = loadedIDs?.Any() != true ? ids : ids.Except(loadedIDs).ToArray();

            return
                (loadedEntites ?? Array.Empty<TEntity>()).Select(x => x.ToWinResult())
                .Concat(
                    notLoadedIDs.Select(x => OperationResult.Fail($"Entity with ID {x} doesn't exist", x.ToString()).WithoutPayload<TEntity>())
                )
                .ToArray()
                ;
        }

        public virtual async Task<OperationResult> DeleteByID(TId id)
            => await cosmosStorageService.Delete<TEntity>(id.ToString());

        public virtual async Task<OperationResult<TId>[]> DeleteByIDs(params TId[] ids)
            => await Task.WhenAll(ids.Select(async id => (await DeleteByID(id)).WithPayload(id)));

        public async Task<OperationResult<Page<TEntity>>> LoadPage(TFilter filter)
        {
            OperationResult<Page<TEntity>> result = OperationResult.Fail("Not yet started").WithoutPayload<Page<TEntity>>();

            await
                new Func<Task>(async () =>
                {
                    Dictionary<string, object> sqlParams = new Dictionary<string, object>();
                    ImACosmosSqlFilterCriteria[] sqlFilters = ApplyFilter(filter, sqlParams);

                    OperationResult<ILimitedEnumerable<TEntity>> pageOperationResult
                        = await cosmosStorageService.LoadByCustomCriteria<TEntity>(
                            sqlFilters: sqlFilters,
                            sqlParams: sqlParams,
                            sortCriteria: filter.ToCosmosSqlSortCriteria(),
                            limitCriteria: filter.ToCosmosSqlLimitCriteria(),
                            partitionKey: typeof(TEntity).ToPartitionKey()
                        );

                    if (!pageOperationResult.IsSuccessful)
                    {
                        result = pageOperationResult.WithoutPayload<Page<TEntity>>();
                        return;
                    }

                    ILimitedEnumerable<TEntity> pageResult = pageOperationResult.Payload;

                    result
                        = Page<TEntity>.For(
                            filter,
                            pageResult.TotalNumberOfItems,
                            pageResult.ToArray()
                        )
                        .ToWinResult();
                })
                .TryOrFailWithGrace(
                    onFail: ex =>
                    {
                        result = OperationResult.Fail(ex, $"Error occurred while trying to load {typeof(TEntity).Name} page. Message: {ex.Message}").WithoutPayload<Page<TEntity>>();
                    }
                );

            return result;
        }

        public async Task<OperationResult<IDisposableEnumerable<TEntity>>> Stream(TFilter filter)
        {
            OperationResult<IDisposableEnumerable<TEntity>> result = OperationResult.Fail("Not yet started").WithoutPayload<IDisposableEnumerable<TEntity>>();

            await
                new Func<Task>(async () =>
                {
                    Dictionary<string, object> sqlParams = new Dictionary<string, object>();
                    ImACosmosSqlFilterCriteria[] sqlFilters = ApplyFilter(filter, sqlParams);

                    OperationResult<IDisposableEnumerable<TEntity>> streamOperationResult
                        = await cosmosStorageService.StreamByCustomCriteria<TEntity>(
                            sqlFilters: sqlFilters,
                            sqlParams: sqlParams,
                            sortCriteria: filter.ToCosmosSqlSortCriteria(),
                            limitCriteria: filter.ToCosmosSqlLimitCriteria(),
                            partitionKey: typeof(TEntity).ToPartitionKey()
                        );

                    if (!streamOperationResult.IsSuccessful)
                    {
                        result = streamOperationResult.WithoutPayload<IDisposableEnumerable<TEntity>>();
                        return;
                    }

                    IDisposableEnumerable<TEntity> streamResult = streamOperationResult.Payload;

                    result
                        = streamResult
                        .ToWinResult();
                })
                .TryOrFailWithGrace(
                    onFail: ex =>
                    {
                        result = OperationResult.Fail(ex, $"Error occurred while trying to stream filtered {typeof(TEntity).Name}. Message: {ex.Message}").WithoutPayload<IDisposableEnumerable<TEntity>>();
                    }
                );

            return result;
        }

        public async Task<OperationResult<IDisposableEnumerable<TEntity>>> StreamAll()
        {
            OperationResult<IDisposableEnumerable<TEntity>> result = OperationResult.Fail("Not yet started").WithoutPayload<IDisposableEnumerable<TEntity>>();

            await
                new Func<Task>(async () =>
                {
                    OperationResult<IDisposableEnumerable<TEntity>> streamOperationResult
                        = await cosmosStorageService.StreamAll<TEntity>(partitionKey: typeof(TEntity).ToPartitionKey());

                    if (!streamOperationResult.IsSuccessful)
                    {
                        result = streamOperationResult.WithoutPayload<IDisposableEnumerable<TEntity>>();
                        return;
                    }

                    IDisposableEnumerable<TEntity> streamResult = streamOperationResult.Payload;

                    result
                        = streamResult
                        .ToWinResult();
                })
                .TryOrFailWithGrace(
                    onFail: ex =>
                    {
                        result = OperationResult.Fail(ex, $"Error occurred while trying to stream all {typeof(TEntity).Name}. Message: {ex.Message}").WithoutPayload<IDisposableEnumerable<TEntity>>();
                    }
                );

            return result;
        }


        protected virtual ImACosmosSqlFilterCriteria[] ApplyFilter(TFilter filter, IDictionary<string, object> sqlParams)
        {
            return
                filter
                .ToCosmosSqlFilterCriterias(sqlParams, FilterToStoreMapping)
                .ToArrayNullIfEmpty()
                ;
        }

        protected virtual IDictionary<string, string> FilterToStoreMapping => null;
    }
}
