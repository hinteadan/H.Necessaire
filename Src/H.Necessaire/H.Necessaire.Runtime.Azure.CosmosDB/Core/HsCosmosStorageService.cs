using H.Necessaire.Runtime.Azure.CosmosDB.Core.Model;
using H.Necessaire.Runtime.Azure.CosmosDB.Core.Model.SQL;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Azure.CosmosDB.Core
{
    internal class HsCosmosStorageService : ImADependency
    {
        #region Construct
        HsCosmosConfig config;
        public virtual void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            config = ReadDefaultConfigFromRuntimeConfig(dependencyProvider.GetRuntimeConfig());
        }

        public HsCosmosStorageService WithConfig(Func<HsCosmosConfig, HsCosmosConfig> configDecorator)
        {
            if (configDecorator == null)
                return this;
            config = configDecorator.Invoke(config);
            return this;
        }
        #endregion

        public async Task<OperationResult> Save<T>(HsCosmosItem<T> item)
        {
            if (item == null)
                return OperationResult.Fail("Item is NULL");

            OperationResult result = OperationResult.Fail("Not yet started");

            await
                new Func<Task>(async () =>
                {
                    using (HsCosmosSession session = NewSession())
                    {
                        await session.Container.UpsertItemAsync(item);
                    }

                    result = OperationResult.Win();
                })
                .TryOrFailWithGrace(
                    onFail: ex =>
                    {
                        result = OperationResult.Fail(ex, $"Error occurred while trying to save data {item.ID} to Azure CosmosDB. Message: {ex.Message}");
                    }
                );

            return result;
        }

        public async Task<OperationResult<T>> Load<T>(string id, string partitionKey = null)
        {
            if (id.IsEmpty())
                return OperationResult.Fail("ID is empty").WithoutPayload<T>();

            OperationResult<T> result = OperationResult.Fail("Not yet started").WithoutPayload<T>();

            await
                new Func<Task>(async () =>
                {
                    using (HsCosmosSession session = NewSession())
                    {
                        ItemResponse<HsCosmosItem<T>> readResponse =
                            await session.Container.ReadItemAsync<HsCosmosItem<T>>(
                                id,
                                new PartitionKey(partitionKey.NullIfEmpty() ?? typeof(T).ToPartitionKey())
                            );

                        result
                            = readResponse
                            .Resource
                            .Data
                            .ToWinResult()
                            ;

                    }
                })
                .TryOrFailWithGrace(
                    onFail: ex =>
                    {
                        result = OperationResult.Fail(ex, $"Error occurred while trying to load data {id} from Azure CosmosDB. Message: {ex.Message}").WithoutPayload<T>(); ;
                    }
                );

            return result;
        }

        public async Task<OperationResult> Delete<T>(string id, string partitionKey = null)
        {
            if (id.IsEmpty())
                return OperationResult.Fail("ID is empty");

            OperationResult result = OperationResult.Fail("Not yet started");

            await
                new Func<Task>(async () =>
                {
                    using (HsCosmosSession session = NewSession())
                    {
                        await session.Container.DeleteItemAsync<HsCosmosItem<T>>(
                            id,
                            new PartitionKey(partitionKey.NullIfEmpty() ?? typeof(T).ToPartitionKey())
                        );
                    }

                    result = OperationResult.Win();
                })
                .TryOrFailWithGrace(
                    onFail: ex =>
                    {
                        result = OperationResult.Fail(ex, $"Error occurred while trying to delete data {id} from Azure CosmosDB. Message: {ex.Message}");
                    }
                );

            return result;
        }

        public async Task<OperationResult<IDisposableEnumerable<T>>> StreamAll<T>(string partitionKey = null, string dataType = null)
        {
            OperationResult<IDisposableEnumerable<T>> result = OperationResult.Fail("Not yet started").WithoutPayload<IDisposableEnumerable<T>>();

            await
                new Func<Task>(() =>
                {
                    string cosmosSql = "SELECT * FROM docs doc WHERE doc.partitionKey = @partitionKey";
                    if (!dataType.IsEmpty())
                        cosmosSql += " AND doc.dataType = @dataType";


                    QueryDefinition query = new QueryDefinition(cosmosSql);
                    query.WithParameter("@partitionKey", partitionKey.NullIfEmpty() ?? typeof(T).ToPartitionKey());
                    if (!dataType.IsEmpty())
                        query.WithParameter("@dataType", dataType);

                    HsCosmosSession session = NewSession();

                    result =
                        (
                            new HsCosmosFeedEnumerable<T>
                            (
                                session.Container.GetItemQueryIterator<HsCosmosItem<T>>(query),
                                session
                            )
                            .ProjectTo(x => x.Data)
                        )
                        .ToWinResult();

                    return Task.CompletedTask;
                })
                .TryOrFailWithGrace(
                    onFail: ex =>
                    {
                        result = OperationResult.Fail(ex, $"Error occurred while trying to stream data from Azure CosmosDB. Message: {ex.Message}").WithoutPayload<IDisposableEnumerable<T>>();
                    }
                );

            return result;
        }

        public async Task<OperationResult<ILimitedEnumerable<T>>> LoadByCustomCriteria<T>(
            ImACosmosSqlFilterCriteria[] sqlFilters,
            IDictionary<string, object> sqlParams,
            CosmosSqlSortCriteria sortCriteria = null,
            CosmosSqlLimitCriteria limitCriteria = null,
            string partitionKey = null,
            string dataType = null
            )
        {
            OperationResult<ILimitedEnumerable<T>> result = OperationResult.Fail("Not yet started").WithoutPayload<ILimitedEnumerable<T>>();

            await
                new Func<Task>(async () =>
                {
                    Dictionary<string, object> actualParams = new Dictionary<string, object>(sqlParams);
                    List<ImACosmosSqlFilterCriteria> actualFilters = sqlFilters?.Any() == true ? new List<ImACosmosSqlFilterCriteria>(sqlFilters) : new List<ImACosmosSqlFilterCriteria>();
                    if (!partitionKey.IsEmpty())
                    {
                        actualFilters.Add(new CosmosSqlFilterCriteria("partitionKey", "="));
                        actualParams.Add("partitionKey", partitionKey);
                    }
                    if (!dataType.IsEmpty())
                    {
                        actualFilters.Add(new CosmosSqlFilterCriteria("dataType", "="));
                        actualParams.Add("dataType", dataType);
                    }

                    string cosmosCountSql = actualFilters.ToCosmosSqlCountQuerySyntax();
                    QueryDefinition countQuery = new QueryDefinition(cosmosCountSql).WithParameters(actualParams);
                    string cosmosSql = actualFilters.ToCosmosSqlQuerySyntax(sortCriteria, limitCriteria);
                    QueryDefinition query = new QueryDefinition(cosmosSql).WithParameters(actualParams);

                    int totalCount = -1;
                    using (HsCosmosSession session = NewSession())
                    {
                        totalCount
                            = (await session
                            .Container
                            .GetItemQueryIterator<int>(
                                countQuery,
                                requestOptions: new QueryRequestOptions { MaxItemCount = 1 }
                            )
                            .ReadNextAsync())
                            .Resource
                            .Single()
                            ;

                        using (IDisposableEnumerable<T> dataStream
                            = new HsCosmosFeedEnumerable<T>(session.Container.GetItemQueryIterator<HsCosmosItem<T>>(query))
                            .ProjectTo(x => x.Data)
                        )
                        {
                            T[] pageData = dataStream.ToArray();

                            result
                                = (new HsCosmosCustomQueryResult<T>(
                                    offset: limitCriteria?.Offset ?? 0,
                                    length: pageData.Length,
                                    totalNumberOfItems: totalCount,
                                    entries: pageData
                                ) as ILimitedEnumerable<T>)
                                .ToWinResult();
                        }
                    }
                })
                .TryOrFailWithGrace(
                    onFail: ex =>
                    {
                        result = OperationResult.Fail(ex, $"Error occurred while trying to load data page from Azure CosmosDB via custom criteria. Message: {ex.Message}").WithoutPayload<ILimitedEnumerable<T>>();
                    }
                );

            return result;
        }

        public async Task<OperationResult<IDisposableEnumerable<T>>> StreamByCustomCriteria<T>(
            ImACosmosSqlFilterCriteria[] sqlFilters,
            IDictionary<string, object> sqlParams,
            CosmosSqlSortCriteria sortCriteria = null,
            CosmosSqlLimitCriteria limitCriteria = null,
            string partitionKey = null,
            string dataType = null
        )
        {
            OperationResult<IDisposableEnumerable<T>> result = OperationResult.Fail("Not yet started").WithoutPayload<IDisposableEnumerable<T>>();

            await
                new Func<Task>(() =>
                {
                    Dictionary<string, object> actualParams = new Dictionary<string, object>(sqlParams);
                    List<ImACosmosSqlFilterCriteria> actualFilters = sqlFilters?.Any() == true ? new List<ImACosmosSqlFilterCriteria>(sqlFilters) : new List<ImACosmosSqlFilterCriteria>();
                    if (!partitionKey.IsEmpty())
                    {
                        actualFilters.Add(new CosmosSqlFilterCriteria("partitionKey", "="));
                        actualParams.Add("partitionKey", partitionKey);
                    }
                    if (!dataType.IsEmpty())
                    {
                        actualFilters.Add(new CosmosSqlFilterCriteria("dataType", "="));
                        actualParams.Add("dataType", dataType);
                    }

                    string cosmosSql = actualFilters.ToCosmosSqlQuerySyntax(sortCriteria, limitCriteria);

                    QueryDefinition query = new QueryDefinition(cosmosSql).WithParameters(actualParams);

                    HsCosmosSession session = NewSession();

                    result =
                        (
                            new HsCosmosFeedEnumerable<T>
                            (
                                session.Container.GetItemQueryIterator<HsCosmosItem<T>>(query),
                                session
                            )
                            .ProjectTo(x => x.Data)
                        )
                        .ToWinResult();

                    return Task.CompletedTask;
                })
                .TryOrFailWithGrace(
                    onFail: ex =>
                    {
                        result = OperationResult.Fail(ex, $"Error occurred while trying to stream data from Azure CosmosDB via custom criteria. Message: {ex.Message}").WithoutPayload<IDisposableEnumerable<T>>();
                    }
                );

            return result;
        }

        public async Task<OperationResult<IDisposableEnumerable<T>>> StreamByCustomSql<T>(string cosmosSql, IDictionary<string, object> sqlParams)
        {
            OperationResult<IDisposableEnumerable<T>> result = OperationResult.Fail("Not yet started").WithoutPayload<IDisposableEnumerable<T>>();

            await
                new Func<Task>(() =>
                {
                    HsCosmosSession session = NewSession();

                    QueryDefinition query = new QueryDefinition(cosmosSql).WithParameters(sqlParams);

                    result =
                        (
                            new HsCosmosFeedEnumerable<T>
                            (
                                session.Container.GetItemQueryIterator<HsCosmosItem<T>>(query),
                                session
                            )
                            .ProjectTo(x => x.Data)
                        )
                        .ToWinResult();

                    return Task.CompletedTask;
                })
                .TryOrFailWithGrace(
                    onFail: ex =>
                    {
                        result = OperationResult.Fail(ex, $"Error occurred while trying to stream data from Azure CosmosDB via custom SQL. Message: {ex.Message}").WithoutPayload<IDisposableEnumerable<T>>();
                    }
                );

            return result;
        }

        public virtual HsCosmosSession NewSession()
        {
            CosmosClient client
                = new CosmosClient(
                    accountEndpoint: config?.AccountEndpoint,
                    authKeyOrResourceToken: config?.PrimaryAccessKey,
                    new CosmosClientOptions
                    {
                        ApplicationName = config?.ApplicationName,
                        Serializer = HsCosmosSerializer.Instance,
                    }
                );
            Database db = client.GetDatabase(config?.DatabaseID);
            Container container = db.GetContainer(config?.ContainerID);

            return
                new HsCosmosSession(
                    cosmosClient: client,
                    cosmosDatabase: db,
                    cosmosContainer: container
                    );
        }

        public virtual HsCosmosConfig ReadDefaultConfigFromRuntimeConfig(RuntimeConfig runtimeConfig)
        {
            ConfigNode azureCosmosDbConfig = runtimeConfig?.Get("AzureCosmosDB");
            if (azureCosmosDbConfig == null)
                return null;

            return
                new HsCosmosConfig
                {
                    AccountEndpoint = azureCosmosDbConfig.Get("URL")?.ToString(),
                    ExtraAccessKeys = azureCosmosDbConfig.Get("Keys")?.GetAllStrings(),
                    DatabaseID = azureCosmosDbConfig.Get("DatabaseID")?.ToString(),
                    ContainerID = azureCosmosDbConfig.Get("ContainerID")?.ToString(),
                }
                .And(cfg =>
                {
                    cfg.PrimaryAccessKey = cfg.ExtraAccessKeys?.FirstOrDefault();
                    cfg.ExtraAccessKeys = cfg.ExtraAccessKeys?.Jump(1);

                    cfg.ApplicationName
                        = azureCosmosDbConfig.Get("App")?.ToString()
                        ?? azureCosmosDbConfig.Get("Application")?.ToString()
                        ?? azureCosmosDbConfig.Get("AppName")?.ToString()
                        ?? azureCosmosDbConfig.Get("ApplicationName")?.ToString()
                        ?? cfg.ApplicationName
                        ;
                })
                ;
        }
    }
}
