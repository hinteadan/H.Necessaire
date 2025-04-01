using Couchbase.Lite;
using Couchbase.Lite.Query;
using H.Necessaire.Couchbase.Lite.Querying;
using H.Necessaire.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CouchExpression = Couchbase.Lite.Query.Expression;

namespace H.Necessaire.Couchbase.Lite
{
    public class CouchbaseOperations : IDisposable
    {
        const string dataBinBlobPropertyName = "DataBinBlob";
        readonly CouchbaseOperationScope operationScope;
        public CouchbaseOperations(CouchbaseOperationScope operationScope)
        {
            this.operationScope = operationScope;
        }

        internal CouchbaseOperationScope Scope => operationScope;

        public void Dispose()
        {
            new Action(operationScope.Dispose).TryOrFailWithGrace();
        }

        public SelectOperationResult<T> SelectAll<T>() => operationScope.SelectAll<T>();
        public SelectOperationResult<T> SelectCount<T>() => operationScope.SelectCount<T>();
        public SelectOperationResult<T> Select<T>(params Expression<Func<T, object>>[] selectors) => operationScope.Select<T>(selectors);
        public SelectOperationResult<T> Select<T>(params ISelectResult[] selects) => operationScope.Select<T>(selects);


        public Task<OperationResult<T>> Save<T, TID>(T document, Func<TID, string> storageIdBuilder = null) where T : IDentityType<TID>
        {
            OperationResult<T> result = OperationResult.Fail("Not yet started").WithPayload(document);

            string documentID = storageIdBuilder?.Invoke(document.ID) ?? document.ID?.ToString();

            new Action(() =>
            {

                using (MutableDocument dbDocument = new MutableDocument(documentID, document.ToJsonObject()))
                {
                    operationScope.Collection.Save(dbDocument);
                }

                result = document.ToWinResult();

            }).TryOrFailWithGrace(onFail: ex =>
            {
                result = OperationResult.Fail(ex, $"Error occurred while trying to save document {documentID} to couchbase collection {operationScope.Collection.FullName}. Reason: {ex.Message}").WithPayload(document);
            });

            return result.AsTask();
        }
        public Task<OperationResult<T>> Delete<T, TID>(TID documentID, Func<TID, string> storageIdBuilder = null, bool isSoftDelete = false) where T : IDentityType<TID>
        {
            OperationResult<T> result = OperationResult.Fail("Not yet started").WithoutPayload<T>();

            string dbDocumentID = storageIdBuilder?.Invoke(documentID) ?? documentID?.ToString();

            new Action(() =>
            {
                Document document = operationScope.Collection.GetDocument(dbDocumentID);

                if (document is null)
                {
                    result = OperationResult.Win($"Document {dbDocumentID} doesn't exist in collection {operationScope.Collection.FullName}, nothing to delete.").WithoutPayload<T>();
                    return;
                }

                using (document)
                {
                    if (isSoftDelete)
                        operationScope.Collection.Delete(document);
                    else
                        operationScope.Collection.Purge(document);

                    result = document.ToJSON().JsonToObject<T>().ToWinResult();
                }

            }).TryOrFailWithGrace(onFail: ex =>
            {
                result = OperationResult.Fail(ex, $"Error occurred while trying to delete document {documentID} from couchbase collection {operationScope.Collection.FullName}. Reason: {ex.Message}").WithoutPayload<T>();
            });

            return result.AsTask();
        }
        public Task<OperationResult<T>> Load<T, TID>(TID documentID, Func<TID, string> storageIdBuilder = null) where T : IDentityType<TID>
        {
            OperationResult<T> result = OperationResult.Fail("Not yet started").WithoutPayload<T>();

            string dbDocumentID = storageIdBuilder?.Invoke(documentID) ?? documentID?.ToString();

            new Action(() =>
            {
                Document document = operationScope.Collection.GetDocument(dbDocumentID);

                if (document is null)
                {
                    result = OperationResult.Fail($"Document {dbDocumentID} doesn't exist in collection {operationScope.Collection.FullName}").WithoutPayload<T>();
                    return;
                }

                using (document)
                {
                    result = document.ToJSON().JsonToObject<T>().ToWinResult();
                }

            }).TryOrFailWithGrace(onFail: ex =>
            {
                result = OperationResult.Fail(ex, $"Error occurred while trying to load document {documentID} from couchbase collection {operationScope.Collection.FullName}. Reason: {ex.Message}").WithoutPayload<T>();
            });

            return result.AsTask();
        }
        public Task<OperationResult<IDisposableEnumerable<T>>> StreamAll<T>()
        {
            OperationResult<IDisposableEnumerable<T>> result = OperationResult.Fail("Not yet started").WithoutPayload<IDisposableEnumerable<T>>();

            new Action(() =>
            {
                IFrom query = QueryBuilder.Select(SelectResult.All()).From(DataSource.Collection(operationScope.Collection));
                IResultSet resultSet = query.Execute();

                IDisposableEnumerable<T> resultPayload = resultSet.ToDisposableEnumerable(resultSet, query, this).ProjectTo(dbRes =>
                {
                    DictionaryObject dbResDict = dbRes.GetDictionary(0);
                    return dbResDict.ToJSON().JsonToObject<T>();
                });

                result = resultPayload.ToWinResult();

            }).TryOrFailWithGrace(onFail: ex =>
            {
                result = OperationResult.Fail(ex, $"Error occurred while trying to stream all documents from couchbase collection {operationScope.Collection.FullName}. Reason: {ex.Message}").WithoutPayload<IDisposableEnumerable<T>>();
            });

            return result.AsTask();
        }
        public Task<OperationResult<IDisposableEnumerable<T>>> StreamQuery<T>(ICouchbaseQuery couchbaseQuery)
        {
            OperationResult<IDisposableEnumerable<T>> result = OperationResult.Fail("Not yet started").WithoutPayload<IDisposableEnumerable<T>>();

            new Action(() =>
            {
                IQuery query = couchbaseQuery.ToQuery();

                IResultSet resultSet = query.Execute();

                IDisposableEnumerable<T> resultPayload = resultSet.ToDisposableEnumerable(resultSet, query, this).ProjectTo(dbRes => MapDbResultsToType<T>(dbRes));

                result = resultPayload.ToWinResult();

            }).TryOrFailWithGrace(onFail: ex =>
            {
                result = OperationResult.Fail(ex, $"Error occurred while trying to stream all documents from couchbase collection {operationScope.Collection.FullName}. Reason: {ex.Message}").WithoutPayload<IDisposableEnumerable<T>>();
            });

            return result.AsTask();
        }
        public Task<OperationResult<Page<T>>> LoadPage<T>(ICouchbaseQuery couchbaseQuery, PageFilter pageFilter)
        {
            OperationResult<Page<T>> result = OperationResult.Fail("Not yet started").WithoutPayload<Page<T>>();

            long totalCount;
            new Action(() =>
            {
                using (IQuery countQuery = couchbaseQuery.ToCountQuery())
                using (IResultSet fullResultsSet = countQuery.Execute())
                {
                    Result firstDbResult = fullResultsSet.FirstOrDefault();
                    if (firstDbResult is null)
                    {
                        result = Page<T>.Empty().ToWinResult();
                        return;
                    }

                    totalCount = firstDbResult.GetLong(0);
                }

                IQuery query = couchbaseQuery.ToQuery();

                if (query is IFrom fromQuery)
                    query = fromQuery.Limit(CouchExpression.Long(pageFilter.PageSize), CouchExpression.Long(pageFilter.PageIndex * pageFilter.PageSize));
                else if (query is IWhere whereQuery)
                    query = whereQuery.Limit(CouchExpression.Long(pageFilter.PageSize), CouchExpression.Long(pageFilter.PageIndex * pageFilter.PageSize));
                else if (query is IGroupBy groupQuery)
                    query = groupQuery.Limit(CouchExpression.Long(pageFilter.PageSize), CouchExpression.Long(pageFilter.PageIndex * pageFilter.PageSize));
                else if (query is IOrderBy orderQuery)
                    query = orderQuery.Limit(CouchExpression.Long(pageFilter.PageSize), CouchExpression.Long(pageFilter.PageIndex * pageFilter.PageSize));
                else
                {
                    query.Dispose();
                    result = OperationResult.Fail("Pagination not supported for the current query").WithoutPayload<Page<T>>();
                    return;
                }

                using (query)
                using (IResultSet resultSet = query.Execute())
                {
                    List<Result> pageDbResults = resultSet.AllResults();

                    T[] results = pageDbResults.Select(dbRes => MapDbResultsToType<T>(dbRes)).ToArray();

                    Page<T> page = Page<T>.For(new ConcretePageFilter(pageFilter), totalCount, results);

                    result = page.ToWinResult();
                }

            }).TryOrFailWithGrace(onFail: ex =>
            {
                result = OperationResult.Fail(ex, $"Error occurred while trying to load page {pageFilter?.PageIndex ?? -1} of size {pageFilter?.PageSize ?? -1} from couchbase collection {operationScope.Collection.FullName}. Reason: {ex.Message}").WithoutPayload<Page<T>>();
            });

            return result.AsTask();
        }


        public async Task<OperationResult<DataBin>> SaveBlob(DataBin dataBin, bool isExplicitBlobScopeProvided = false)
        {

            OperationResult<DataBin> result = OperationResult.Fail("Not yet started").WithPayload(dataBin);
            string documentID = dataBin.ID.ToString();

            CouchbaseOperationScope blobScope = isExplicitBlobScopeProvided ? null : operationScope.DefaultForBlobInCurrentCollection();
            CouchbaseOperationScope actualScope = blobScope ?? operationScope;
            await new Func<Task>(async () =>
            {
                using (MutableDocument dbDocument = new MutableDocument(documentID, dataBin.ToJsonObject()))
                {
                    using (ImADataBinStream dataBinStream = await dataBin.OpenDataBinStream())
                    using (Stream blobStream = dataBinStream.DataStream)
                    {
                        dbDocument.SetBlob(dataBinBlobPropertyName, new Blob(dataBin.Format.MimeType, blobStream));
                        actualScope.Collection.Save(dbDocument);
                    }
                }

                result = dataBin.ToBin(m => OpenBlobStream(m, isExplicitBlobScopeProvided)).ToWinResult();

            }).TryOrFailWithGrace(onFail: ex =>
            {
                result = OperationResult.Fail(ex, $"Error occurred while trying to save DataBin {documentID} to couchbase collection {actualScope.Collection.FullName}. Reason: {ex.Message}").WithPayload(dataBin);
            });

            if (blobScope != null)
                blobScope.Dispose();

            return result;
        }

        public Task<OperationResult<DataBin>> LoadBlob(Guid id, bool isExplicitBlobScopeProvided = false)
        {
            OperationResult<DataBin> result = OperationResult.Fail("Not yet started").WithoutPayload<DataBin>();

            string dbDocumentID = id.ToString();

            CouchbaseOperationScope blobScope = isExplicitBlobScopeProvided ? null : operationScope.DefaultForBlobInCurrentCollection();
            CouchbaseOperationScope actualScope = blobScope ?? operationScope;
            new Action(() =>
            {
                Document document = actualScope.Collection.GetDocument(dbDocumentID);

                if (document is null)
                {
                    result = OperationResult.Fail($"Blob {dbDocumentID} doesn't exist in collection {actualScope.Collection.FullName}").WithoutPayload<DataBin>();
                    return;
                }

                using (document)
                {
                    result = document.ToJSON().JsonToObject<DataBinMeta>().ToBin(m => OpenBlobStream(m, isExplicitBlobScopeProvided)).ToWinResult();
                }

            }).TryOrFailWithGrace(onFail: ex =>
            {
                result = OperationResult.Fail(ex, $"Error occurred while trying to load blob {id} from couchbase collection {actualScope.Collection.FullName}. Reason: {ex.Message}").WithoutPayload<DataBin>();
            });
            if (blobScope != null)
                blobScope.Dispose();

            return result.AsTask();
        }

        public Task<OperationResult<DataBinMeta>> DeleteBlob(Guid id, bool isExplicitBlobScopeProvided = false, bool isSoftDelete = false, bool isCompactHandledManually = false)
        {
            OperationResult<DataBinMeta> result = OperationResult.Fail("Not yet started").WithoutPayload<DataBinMeta>();

            string dbDocumentID = id.ToString();

            CouchbaseOperationScope blobScope = isExplicitBlobScopeProvided ? null : operationScope.DefaultForBlobInCurrentCollection();
            CouchbaseOperationScope actualScope = blobScope ?? operationScope;
            new Action(() =>
            {
                Document document = actualScope.Collection.GetDocument(dbDocumentID);

                if (document is null)
                {
                    result = OperationResult.Win($"DataBin {dbDocumentID} doesn't exist in collection {actualScope.Collection.FullName}, nothing to delete.").WithoutPayload<DataBinMeta>();
                    return;
                }

                using (document)
                {
                    if (isSoftDelete)
                        actualScope.Collection.Delete(document);
                    else
                        actualScope.Collection.Purge(document);

                    if (!isCompactHandledManually)
                        actualScope.Database.PerformMaintenance(MaintenanceType.Compact);

                    result = document.ToJSON().JsonToObject<DataBinMeta>().ToWinResult();
                }

            }).TryOrFailWithGrace(onFail: ex =>
            {
                result = OperationResult.Fail(ex, $"Error occurred while trying to delete blob {id} from couchbase collection {actualScope.Collection.FullName}. Reason: {ex.Message}").WithoutPayload<DataBinMeta>();
            });
            if (blobScope != null)
                blobScope.Dispose();

            return result.AsTask();
        }

        public Task<OperationResult> PerformDatabaseCompactMaintenance()
        {
            OperationResult result = OperationResult.Fail("Not yet started");

            new Action(() =>
            {
                operationScope.Database.PerformMaintenance(MaintenanceType.Compact);

                result = OperationResult.Win();

            })
            .TryOrFailWithGrace(
                onFail: ex => result = OperationResult.Fail(ex, $"Error occurred while tryinh to Perform Database Compact Maintenance. Reason: {ex.Message}")
            );

            return result.AsTask();
        }

        public Task<OperationResult<IDisposableEnumerable<DataBin>>> StreamAllBlobs(bool isExplicitBlobScopeProvided = false)
        {
            OperationResult<IDisposableEnumerable<DataBin>> result = OperationResult.Fail("Not yet started").WithoutPayload<IDisposableEnumerable<DataBin>>();

            CouchbaseOperationScope blobScope = isExplicitBlobScopeProvided ? null : operationScope.DefaultForBlobInCurrentCollection();
            CouchbaseOperationScope actualScope = blobScope ?? operationScope;
            new Action(() =>
            {
                IFrom query = QueryBuilder.Select(SelectResult.All()).From(DataSource.Collection(actualScope.Collection));
                IResultSet resultSet = query.Execute();

                IDisposableEnumerable<DataBin> resultPayload = resultSet.ToDisposableEnumerable(resultSet, query, blobScope, this).ProjectTo(dbRes =>
                {
                    DictionaryObject dbResDict = dbRes.GetDictionary(0);
                    return dbResDict.ToJSON().JsonToObject<DataBinMeta>().ToBin(m => OpenBlobStream(m, isExplicitBlobScopeProvided));
                });

                result = resultPayload.ToWinResult();

            }).TryOrFailWithGrace(onFail: ex =>
            {
                result = OperationResult.Fail(ex, $"Error occurred while trying to stream blobs from couchbase collection {actualScope.Collection.FullName}. Reason: {ex.Message}").WithoutPayload<IDisposableEnumerable<DataBin>>();
            });

            return result.AsTask();
        }
        public Task<OperationResult<IDisposableEnumerable<DataBin>>> StreamBlobsQuery(ICouchbaseQuery couchbaseQuery, bool isExplicitBlobScopeProvided = false)
        {
            OperationResult<IDisposableEnumerable<DataBin>> result = OperationResult.Fail("Not yet started").WithoutPayload<IDisposableEnumerable<DataBin>>();

            CouchbaseOperationScope blobScope = isExplicitBlobScopeProvided ? null : operationScope.DefaultForBlobInCurrentCollection();
            CouchbaseOperationScope actualScope = blobScope ?? operationScope;
            new Action(() =>
            {
                IQuery query = couchbaseQuery.ToQuery();

                IResultSet resultSet = query.Execute();

                IDisposableEnumerable<DataBin> resultPayload = resultSet.ToDisposableEnumerable(resultSet, query, blobScope, this).ProjectTo(dbRes => MapDbResultsToType<DataBinMeta>(dbRes).ToBin(m => OpenBlobStream(m, isExplicitBlobScopeProvided)));

                result = resultPayload.ToWinResult();

            }).TryOrFailWithGrace(onFail: ex =>
            {
                result = OperationResult.Fail(ex, $"Error occurred while trying to stream blobs from couchbase collection {actualScope.Collection.FullName}. Reason: {ex.Message}").WithoutPayload<IDisposableEnumerable<DataBin>>();
            });

            return result.AsTask();
        }
        public Task<OperationResult<Page<DataBin>>> LoadBlobsPage(ICouchbaseQuery couchbaseQuery, PageFilter pageFilter, bool isExplicitBlobScopeProvided = false)
        {
            OperationResult<Page<DataBin>> result = OperationResult.Fail("Not yet started").WithoutPayload<Page<DataBin>>();

            long totalCount;

            CouchbaseOperationScope blobScope = isExplicitBlobScopeProvided ? null : operationScope.DefaultForBlobInCurrentCollection();
            CouchbaseOperationScope actualScope = blobScope ?? operationScope;
            new Action(() =>
            {
                using (IQuery countQuery = couchbaseQuery.ToCountQuery())
                using (IResultSet fullResultsSet = countQuery.Execute())
                {
                    Result firstDbResult = fullResultsSet.FirstOrDefault();
                    if (firstDbResult is null)
                    {
                        result = Page<DataBin>.Empty().ToWinResult();
                        return;
                    }

                    totalCount = firstDbResult.GetLong(0);
                }

                IQuery query = couchbaseQuery.ToQuery();

                if (query is IFrom fromQuery)
                    query = fromQuery.Limit(CouchExpression.Long(pageFilter.PageSize), CouchExpression.Long(pageFilter.PageIndex * pageFilter.PageSize));
                else if (query is IWhere whereQuery)
                    query = whereQuery.Limit(CouchExpression.Long(pageFilter.PageSize), CouchExpression.Long(pageFilter.PageIndex * pageFilter.PageSize));
                else if (query is IGroupBy groupQuery)
                    query = groupQuery.Limit(CouchExpression.Long(pageFilter.PageSize), CouchExpression.Long(pageFilter.PageIndex * pageFilter.PageSize));
                else if (query is IOrderBy orderQuery)
                    query = orderQuery.Limit(CouchExpression.Long(pageFilter.PageSize), CouchExpression.Long(pageFilter.PageIndex * pageFilter.PageSize));
                else
                {
                    query.Dispose();
                    result = OperationResult.Fail("Pagination not supported for the current query").WithoutPayload<Page<DataBin>>();
                    return;
                }

                using (query)
                using (IResultSet resultSet = query.Execute())
                {
                    List<Result> pageDbResults = resultSet.AllResults();

                    DataBin[] results = pageDbResults.Select(dbRes => MapDbResultsToType<DataBinMeta>(dbRes).ToBin(m => OpenBlobStream(m, isExplicitBlobScopeProvided))).ToArray();

                    Page<DataBin> page = Page<DataBin>.For(new ConcretePageFilter(pageFilter), totalCount, results);

                    result = page.ToWinResult();
                }

            }).TryOrFailWithGrace(onFail: ex =>
            {
                result = OperationResult.Fail(ex, $"Error occurred while trying to load blob page {pageFilter?.PageIndex ?? -1} of size {pageFilter?.PageSize ?? -1} from couchbase collection {actualScope.Collection.FullName}. Reason: {ex.Message}").WithoutPayload<Page<DataBin>>();
            });
            if (blobScope != null)
                blobScope.Dispose();

            return result.AsTask();
        }

        Task<ImADataBinStream> OpenBlobStream(DataBinMeta meta, bool isExplicitBlobScopeProvided = false)
        {
            CouchbaseOperationScope dbScope = isExplicitBlobScopeProvided ? operationScope.New() : operationScope.DefaultForBlobInCurrentCollection();
            Document doc = dbScope.Collection.GetDocument(meta.ID.ToString());
            return doc.GetBlob(dataBinBlobPropertyName).ContentStream.ToDataBinStream(doc, dbScope).AsTask();
        }

        static T MapDbResultsToType<T>(Result dbRes)
        {
            DictionaryObject dbResDict = dbRes.GetDictionary(0);

            if (dbResDict is null)
            {
                if (typeof(T).IsPrimitive || typeof(T).In(typeof(string), typeof(DateTime), typeof(DateTime?), typeof(decimal), typeof(decimal?)))
                    return (T)dbRes.GetValue(0);


                return dbRes.ToJSON().JsonToObject<T>();
            }

            return dbResDict.ToJSON().JsonToObject<T>();
        }

        private class ConcretePageFilter : IPageFilter
        {
            public ConcretePageFilter(PageFilter pageFilter)
            {
                PageFilter = pageFilter;
            }
            public PageFilter PageFilter { get; }
        }
    }
}
