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
        protected RavenDbStorageResourceBase()
        {
            EnsureIndexes();
        }

        RavenDbDocumentStore ravenDbDocumentStore;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            ravenDbDocumentStore = dependencyProvider.Get<RavenDbDocumentStore>();
        }
        #endregion

        protected abstract string DatabaseName { get; }

        protected abstract TKey GetIdFor(TEntity item);

        protected abstract IRavenQueryable<TEntity> ApplyFilter(IRavenQueryable<TEntity> query, TFilter filter);

        protected virtual string GetIdForRaven(TKey key) => key?.ToString();

        protected virtual Task EnsureIndexes() { return true.AsTask(); }

        public async Task Save(TEntity item)
        {
            await EnsureIndexes();

            using (IAsyncDocumentSession session = NewWriteSession())
            {
                string id = GetIdForRaven(GetIdFor(item));

                bool exists = await session.Advanced.ExistsAsync(id);

                if (exists)
                {
                    session.Delete(id);
                    await session.SaveChangesAsync();
                }

                await session.StoreAsync(item, id);

                await session.SaveChangesAsync();
            }
        }

        public async Task<TEntity> Load(TKey id)
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

        public async Task Delete(TKey id)
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

        public async Task<TEntity[]> Search(TFilter filter)
        {
            await EnsureIndexes();

            using (IAsyncDocumentSession session = NewReadSession())
            {
                IRavenQueryable<TEntity> query = ApplyFilter(session.Query<TEntity, TFilterIndex>(), filter);
                return await query.ToArrayAsync();
            }
        }

        public async Task<IAsyncDocumentSession> CustomQuerySession()
        {
            await EnsureIndexes();

            return NewReadSession();
        }

        public async Task<long> Count(TFilter filter)
        {
            await EnsureIndexes();

            using (IAsyncDocumentSession session = NewReadSession())
            {
                IRavenQueryable<TEntity> query = ApplyFilter(session.Query<TEntity, TFilterIndex>(), filter);
                return await query.CountAsync();
            }
        }

        public async Task<long> DeleteMany(TFilter filter)
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

        private IAsyncDocumentSession NewWriteSession()
        {
            return ravenDbDocumentStore.Store
                                .OpenAsyncSession(
                                    new SessionOptions
                                    {
                                        Database = DatabaseName
                                    }
                                );
        }

        private IAsyncDocumentSession NewReadSession()
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

        protected async Task EnsureIndex(Func<AbstractIndexCreationTask<TEntity>> indexDefiniton)
        {
            await EnsureDatabaseExists();

            await indexDefiniton().ExecuteAsync(ravenDbDocumentStore.Store, ravenDbDocumentStore.Store.Conventions, DatabaseName);
        }

        protected async Task EnsureIndex<TReduceResult>(Func<AbstractIndexCreationTask<TEntity, TReduceResult>> indexDefiniton)
        {
            await EnsureDatabaseExists();

            await indexDefiniton().ExecuteAsync(ravenDbDocumentStore.Store, ravenDbDocumentStore.Store.Conventions, DatabaseName);
        }
    }
}
