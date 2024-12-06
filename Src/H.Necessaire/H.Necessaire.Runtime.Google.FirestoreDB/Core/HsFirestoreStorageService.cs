using Google.Cloud.Firestore;
using H.Necessaire.Runtime.Google.FirestoreDB.Core.Model;
using H.Necessaire.Runtime.Google.FirestoreDB.Core.Model.Querying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Core
{
    public class HsFirestoreStorageService : ImADependency
    {
        #region Construct
        HsFirestoreConfig config;
        protected static FirestoreDb firestoreDB;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            config = ReadDefaultConfigFromRuntimeConfig(dependencyProvider.GetRuntimeConfig());
            if(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS").IsEmpty())
            {
                string googleAuthJsonPath = dependencyProvider.GetRuntimeConfig()?.Get("Google")?.Get("Firestore")?.Get("Auth")?.Get("Json")?.ToString();
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", googleAuthJsonPath);
            }
            firestoreDB = firestoreDB ?? FirestoreDb.Create(config.ProjectID);
        }

        public HsFirestoreStorageService WithConfig(Func<HsFirestoreConfig, HsFirestoreConfig> configDecorator)
        {
            if (configDecorator == null)
                return this;
            config = configDecorator.Invoke(config);
            return this;
        }
        #endregion

        public async Task<OperationResult> Save(HsFirestoreDocument doc, string collectionName = null)
        {
            if (doc == null)
                return OperationResult.Fail("Document is NULL");

            OperationResult result = OperationResult.Fail("Not yet started");

            await
                new Func<Task>(async () =>
                {
                    CollectionReference collection = firestoreDB.Collection(GetCollectionNameFor(doc, collectionName));
                    DocumentReference docRef = collection.Document(doc.ID);
                    DocumentSnapshot existingDocSnap = await docRef.GetSnapshotAsync();
                    if (!existingDocSnap.Exists)
                        await docRef.CreateAsync(doc);
                    else
                        await docRef.SetAsync(doc, SetOptions.Overwrite);
                    result = OperationResult.Win();
                })
                .TryOrFailWithGrace(
                    onFail: ex =>
                    {
                        result = OperationResult.Fail(ex, $"Error occurred while trying to save document {doc.ID} to Google FirestoreDB. Message: {ex.Message}");
                    }
                );

            return result;
        }

        public async Task<OperationResult<HsFirestoreDocument>> Load(string id, string collectionName = null)
        {
            if (id.IsEmpty())
                return OperationResult.Fail("ID is empty").WithoutPayload<HsFirestoreDocument>();

            OperationResult<HsFirestoreDocument> result = OperationResult.Fail("Not yet started").WithoutPayload<HsFirestoreDocument>();

            await
                new Func<Task>(async () =>
                {
                    CollectionReference collection = firestoreDB.Collection(GetCollectionNameFor(doc: null, collectionName));
                    DocumentReference docRef = collection.Document(id);
                    DocumentSnapshot existingDocSnap = await docRef.GetSnapshotAsync();
                    if (!existingDocSnap.Exists)
                    {
                        result = (null as HsFirestoreDocument).ToWinResult();
                        return;
                    }

                    result = existingDocSnap.ConvertTo<HsFirestoreDocument>().ToWinResult();
                })
                .TryOrFailWithGrace(
                    onFail: ex =>
                    {
                        result = OperationResult.Fail(ex, $"Error occurred while trying to load document {id} from Google FirestoreDB. Message: {ex.Message}").WithoutPayload<HsFirestoreDocument>();
                    }
                );

            return result;
        }

        public async Task<OperationResult> Delete(string id, string collectionName = null)
        {
            if (id.IsEmpty())
                return OperationResult.Fail("ID is empty");

            OperationResult result = OperationResult.Fail("Not yet started");

            await
                new Func<Task>(async () =>
                {
                    CollectionReference collection = firestoreDB.Collection(GetCollectionNameFor(doc: null, collectionName));
                    DocumentReference docRef = collection.Document(id);
                    await docRef.DeleteAsync();
                    result = OperationResult.Win();
                })
                .TryOrFailWithGrace(
                    onFail: ex =>
                    {
                        result = OperationResult.Fail(ex, $"Error occurred while trying to delete document {id} from Google FirestoreDB. Message: {ex.Message}");
                    }
                );

            return result;
        }

        public async Task<OperationResult> DeleteBatch(IEnumerable<string> ids, string collectionName = null)
        {
            string[] validIDs = ids?.Select(id => id.NullIfEmpty()).ToNoNullsArray();

            if (validIDs?.Any() != true)
                return OperationResult.Win();

            OperationResult result = OperationResult.Fail("Not yet started");

            await
                new Func<Task>(async () =>
                {
                    CollectionReference collection = firestoreDB.Collection(GetCollectionNameFor(doc: null, collectionName));

                    WriteBatch batchWriter = firestoreDB.StartBatch();
                    foreach (string id in validIDs)
                    {
                        DocumentReference docRef = collection.Document(id);
                        batchWriter.Delete(docRef);
                    }
                    await batchWriter.CommitAsync();

                    result = OperationResult.Win();
                })
                .TryOrFailWithGrace(
                    onFail: ex =>
                    {
                        result = OperationResult.Fail(ex, $"Error occurred while trying to delete a batch of {validIDs.Length} document(s) from Google FirestoreDB. Message: {ex.Message}");
                    }
                );

            return result;
        }

        public async Task<OperationResult<IDisposableEnumerable<HsFirestoreDocument>>> StreamAll(string collectionName = null)
        {
            OperationResult<IDisposableEnumerable<HsFirestoreDocument>> result = OperationResult.Fail("Not yet started").WithoutPayload<IDisposableEnumerable<HsFirestoreDocument>>();

            collectionName = GetCollectionNameFor(doc: null, collectionName);

            await
                new Func<Task>(async () =>
                {
                    CollectionReference collection = firestoreDB.Collection(collectionName);
                    QuerySnapshot all = await collection.GetSnapshotAsync();
                    result
                        = (
                            new HsFirestoreQuerySnapshotEnumerable(all)
                            as IDisposableEnumerable<HsFirestoreDocument>
                        )
                        .ToWinResult();

                })
                .TryOrFailWithGrace(
                    onFail: ex =>
                    {
                        result = OperationResult.Fail(ex, $"Error occurred while trying to stream all {collectionName} documents from Google FirestoreDB. Message: {ex.Message}").WithoutPayload<IDisposableEnumerable<HsFirestoreDocument>>();
                    }
                );

            return result;
        }

        public async Task<OperationResult<IDisposableEnumerable<HsFirestoreDocument>>> StreamByCustomCriteria
        (
            IEnumerable<ImAFirestoreCriteria> criterias,
            HsFirestoreSortCriteria sortCriteria = null,
            HsFirestoreLimitCriteria limitCriteria = null,
            string collectionName = null,
            HsFirestoreCompositionOperator @operator = HsFirestoreCompositionOperator.And
        )
        {
            OperationResult<IDisposableEnumerable<HsFirestoreDocument>> result = OperationResult.Fail("Not yet started").WithoutPayload<IDisposableEnumerable<HsFirestoreDocument>>();

            collectionName = GetCollectionNameFor(doc: null, collectionName);

            await
                new Func<Task>(async () =>
                {
                    CollectionReference collection = firestoreDB.Collection(collectionName);
                    Query query = collection;
                    Filter filter = new HsFirestoreCompositionCriteria { Operator = @operator, Criterias = criterias?.ToNoNullsArray() }.ToFilter();
                    if (filter != null)
                        query = query.Where(filter);
                    if (sortCriteria != null)
                        query
                            = sortCriteria.Direction == HsFirestoreSortCriteria.SortDirection.Ascending
                            ? query.OrderBy(sortCriteria.Property)
                            : query.OrderByDescending(sortCriteria.Property)
                            ;
                    if (limitCriteria != null)
                        query = query.Offset(limitCriteria.Offset).Limit(limitCriteria.Count);
                    QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

                    result
                        = (
                            new HsFirestoreQuerySnapshotEnumerable(querySnapshot)
                            as IDisposableEnumerable<HsFirestoreDocument>
                        )
                        .ToWinResult();

                })
                .TryOrFailWithGrace(
                    onFail: ex =>
                    {
                        result = OperationResult.Fail(ex, $"Error occurred while trying to stream {collectionName} documents by custom criteria from Google FirestoreDB. Message: {ex.Message}").WithoutPayload<IDisposableEnumerable<HsFirestoreDocument>>();
                    }
                );

            return result;
        }

        public async Task<OperationResult<ILimitedEnumerable<HsFirestoreDocument>>> LoadByCustomCriteria
        (
            IEnumerable<ImAFirestoreCriteria> criterias,
            HsFirestoreSortCriteria sortCriteria = null,
            HsFirestoreLimitCriteria limitCriteria = null,
            string collectionName = null,
            HsFirestoreCompositionOperator @operator = HsFirestoreCompositionOperator.And
        )
        {
            OperationResult<ILimitedEnumerable<HsFirestoreDocument>> result = OperationResult.Fail("Not yet started").WithoutPayload<ILimitedEnumerable<HsFirestoreDocument>>();

            collectionName = GetCollectionNameFor(doc: null, collectionName);

            await
                new Func<Task>(async () =>
                {
                    CollectionReference collection = firestoreDB.Collection(collectionName);
                    Query query = collection;
                    Filter filter = new HsFirestoreCompositionCriteria { Operator = @operator, Criterias = criterias?.ToNoNullsArray() }.ToFilter();
                    if (filter != null)
                        query = query.Where(filter);
                    long totalCount = (await query.Count().GetSnapshotAsync()).Count.Value;
                    if (sortCriteria != null)
                        query
                            = sortCriteria.Direction == HsFirestoreSortCriteria.SortDirection.Ascending
                            ? query.OrderBy(sortCriteria.Property)
                            : query.OrderByDescending(sortCriteria.Property)
                            ;
                    if (limitCriteria != null)
                        query = query.Offset(limitCriteria.Offset).Limit(limitCriteria.Count);
                    QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

                    using (IDisposableEnumerable<HsFirestoreDocument> dataStream = new HsFirestoreQuerySnapshotEnumerable(querySnapshot))
                    {
                        HsFirestoreDocument[] pageData = dataStream.ToArray();

                        result
                                = (new HsFirestoreCustomQueryResult(
                                    offset: limitCriteria?.Offset ?? 0,
                                    length: pageData.Length,
                                    totalNumberOfItems: totalCount,
                                    entries: pageData
                                ) as ILimitedEnumerable<HsFirestoreDocument>)
                                .ToWinResult();
                    }

                })
                .TryOrFailWithGrace(
                    onFail: ex =>
                    {
                        result = OperationResult.Fail(ex, $"Error occurred while trying to load {collectionName} documents by custom criteria from Google FirestoreDB. Message: {ex.Message}").WithoutPayload<ILimitedEnumerable<HsFirestoreDocument>>();
                    }
                );

            return result;
        }


        protected virtual HsFirestoreConfig ReadDefaultConfigFromRuntimeConfig(RuntimeConfig runtimeConfig)
        {
            ConfigNode googleFirestoreDbConfig = runtimeConfig?.Get("Google")?.Get("Firestore");
            if (googleFirestoreDbConfig == null)
                return null;

            return
                new HsFirestoreConfig
                {
                    ProjectID = googleFirestoreDbConfig.Get("ProjectID")?.ToString(),
                    CollectionName = googleFirestoreDbConfig.Get("HNecessaireDefault")?.ToString() ?? HsFirestoreConfig.DefaultCollectionName,
                }
                ;
        }

        private string GetCollectionNameFor(HsFirestoreDocument doc, string collectionName)
        {
            if (!collectionName.IsEmpty())
                return collectionName;

            if (!(doc?.DataTypeShortName).IsEmpty())
                return doc.DataTypeShortName;

            string typeName = doc?.Data?.GetType()?.Name;

            if (!typeName.IsEmpty())
                return typeName;

            return
                config?.CollectionName
                ??
                HsFirestoreConfig.DefaultCollectionName
                ;
        }
    }
}
