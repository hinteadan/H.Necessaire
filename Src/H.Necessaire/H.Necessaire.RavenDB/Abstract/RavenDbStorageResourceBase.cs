using Raven.Client.Documents;
using Raven.Client.Documents.Commands;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Operations;
using Raven.Client.Documents.Session;
using Raven.Client.Exceptions;
using Raven.Client.Exceptions.Database;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace H.Necessaire.RavenDB
{
    public abstract class RavenDbStorageResourceBase<TKey, TEntity, TFilter, TFilterIndex> : ImARavenDbStorageResource<TKey, TEntity, TFilter, TFilterIndex>, ImADependency
        where TFilterIndex : AbstractIndexCreationTask<TEntity>, new()
    {
        #region Construct
        string defaultDatabaseName = null;

        RavenDbDocumentStore ravenDbDocumentStore;
        public virtual void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            defaultDatabaseName = dependencyProvider?.GetRuntimeConfig()?.Get("RavenDbConnections")?.Get("DatabaseNames")?.Get("Core")?.ToString();

            ravenDbDocumentStore = dependencyProvider.Get<RavenDbDocumentStore>();

            EnsureIndexes();
        }
        #endregion

        protected virtual string DatabaseName => !string.IsNullOrWhiteSpace(defaultDatabaseName) ? defaultDatabaseName : typeof(TEntity).TypeName().ToSafeFileName();

        protected abstract TKey GetIdFor(TEntity item);

        protected abstract IRavenQueryable<TEntity> ApplyFilter(IRavenQueryable<TEntity> query, TFilter filter);

        protected virtual string GetIdForRaven(TKey key) => key?.ToString();

        protected virtual Task EnsureIndexes() { return true.AsTask(); }

        public virtual async Task Save(TEntity item)
        {
            await EnsureIndexes();

            using (IAsyncDocumentSession session = NewWriteSession())
            {
                string id = GetIdForRaven(GetIdFor(item));

                await session.StoreAsync(item, changeVector: null, id);

                await session.SaveChangesAsync();
            }
        }

        public virtual async Task<TEntity> Load(TKey id)
        {
            await EnsureIndexes();

            using (IAsyncDocumentSession session = NewReadSession())
            {
                string ravenId = GetIdForRaven(id);

                bool exists = await session.Advanced.ExistsAsync(ravenId);

                if (!exists)
                    return default;

                return await session.LoadAsync<TEntity>(ravenId);
            }
        }

        public virtual async Task Delete(TKey id)
        {
            await EnsureIndexes();

            using (IAsyncDocumentSession session = NewWriteSession())
            {
                string ravenId = GetIdForRaven(id);

                bool exists = await session.Advanced.ExistsAsync(ravenId);

                if (!exists)
                    return;

                session.Delete(ravenId);

                await session.SaveChangesAsync();
            }
        }

        public virtual async Task<TEntity[]> Search(TFilter filter)
        {
            await EnsureIndexes();

            using (IAsyncDocumentSession session = NewReadSession())
            {
                IRavenQueryable<TEntity> query = ApplyFilter(session.Query<TEntity, TFilterIndex>(), filter);
                return await query.ToArrayAsync();
            }
        }

        public virtual async Task<IAsyncDocumentSession> CustomQuerySession()
        {
            await EnsureIndexes();

            return NewReadSession();
        }

        public virtual async Task<long> Count(TFilter filter)
        {
            await EnsureIndexes();

            using (IAsyncDocumentSession session = NewReadSession())
            {
                IRavenQueryable<TEntity> query = ApplyFilter(session.Query<TEntity, TFilterIndex>(), filter);
                return await query.CountAsync();
            }
        }

        public virtual async Task<long> DeleteMany(TFilter filter)
        {
            await EnsureIndexes();

            using (IAsyncDocumentSession session = NewWriteSession())
            {
                IRavenQueryable<TEntity> query = ApplyFilter(session.Query<TEntity, TFilterIndex>(), filter);

                StreamQueryStatistics streamQueryStats;
                IAsyncEnumerator<StreamResult<TEntity>> stream = await session.Advanced.StreamAsync(query, out streamQueryStats);

                while (await stream.MoveNextAsync())
                {
                    StreamResult<TEntity> entity = stream.Current;

                    session.Delete(entity.Id);
                }

                await session.SaveChangesAsync();

                return streamQueryStats.TotalResults;
            }
        }

        protected IAsyncDocumentSession NewWriteSession()
        {
            return ravenDbDocumentStore.Store
                                .OpenAsyncSession(
                                    new SessionOptions
                                    {
                                        Database = DatabaseName
                                    }
                                );
        }

        protected IDocumentSession NewSyncWriteSession()
        {
            return ravenDbDocumentStore.Store
                                .OpenSession(
                                    new SessionOptions
                                    {
                                        Database = DatabaseName
                                    }
                                );
        }

        protected IAsyncDocumentSession NewReadSession()
        {
            return ravenDbDocumentStore.Store
                                .OpenAsyncSession(
                                    new SessionOptions
                                    {
                                        Database = DatabaseName,
                                        NoTracking = true,
                                    }
                                );
        }

        protected IDocumentSession NewSyncReadSession()
        {
            return ravenDbDocumentStore.Store
                                .OpenSession(
                                    new SessionOptions
                                    {
                                        Database = DatabaseName,
                                        NoTracking = true,
                                    }
                                );
        }

        private async Task EnsureDatabaseExists()
        {
            if (string.IsNullOrWhiteSpace(DatabaseName))
                throw new InvalidOperationException("Database Name cannot be empty");

            try
            {
                await ravenDbDocumentStore.Store.Maintenance.ForDatabase(DatabaseName).SendAsync(new GetStatisticsOperation());
            }
            catch (DatabaseDoesNotExistException)
            {
                try
                {
                    await ravenDbDocumentStore.Store.Maintenance.Server.SendAsync(new CreateDatabaseOperation(new DatabaseRecord(DatabaseName)));
                }
                catch (ConcurrencyException)
                {
                }
            }
        }

        protected virtual async Task EnsureIndex(Func<AbstractIndexCreationTask<TEntity>> indexDefiniton)
        {
            await EnsureDatabaseExists();

            await indexDefiniton().ExecuteAsync(ravenDbDocumentStore.Store, ravenDbDocumentStore.Store.Conventions, DatabaseName);
        }

        protected virtual async Task EnsureIndex<TReduceResult>(Func<AbstractIndexCreationTask<TEntity, TReduceResult>> indexDefiniton)
        {
            await EnsureDatabaseExists();

            await indexDefiniton().ExecuteAsync(ravenDbDocumentStore.Store, ravenDbDocumentStore.Store.Conventions, DatabaseName);
        }
    }
}
