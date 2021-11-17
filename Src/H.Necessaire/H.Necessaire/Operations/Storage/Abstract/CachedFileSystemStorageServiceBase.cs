using H.Necessaire.Operations.Concrete;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public abstract class CachedFileSystemStorageServiceBase<TId, TEntity, TFilter> : FileSystemStorageServiceBase<TId, TEntity, TFilter>
        where TFilter : IPageFilter, ISortFilter
        where TEntity : IDentityType<TId>
    {
        #region Construct
        readonly InMemoryStorageService<TId, TEntity, TFilter> inMemoryStorage;
        protected CachedFileSystemStorageServiceBase(DirectoryInfo rootFolder = null, string fileExtension = "json")
            : base(rootFolder, fileExtension)
        {
            this.inMemoryStorage = new InMemoryStorageService<TId, TEntity, TFilter>(ApplyFilter);
        }
        #endregion

        public override async Task<OperationResult<TEntity>> LoadByID(TId id)
        {
            OperationResult<TEntity> inMemoryEntry = (await inMemoryStorage.LoadByID(id));

            if (inMemoryEntry.Payload != null)
                return inMemoryEntry;

            OperationResult<TEntity> diskEntry = await base.LoadByID(id);
            if (!diskEntry.IsSuccessful)
                return diskEntry;

            await inMemoryStorage.Save(diskEntry.Payload);

            return diskEntry;
        }

        public override Task<OperationResult<TEntity>[]> LoadByIDs(params TId[] ids)
        {
            return
                Task.WhenAll(
                    ids.Select(
                        id => LoadByID(id)
                    )
                );
        }

        public override Task<OperationResult> DeleteByID(TId id)
        {
            return
                Task.WhenAll(
                    inMemoryStorage.DeleteByID(id),
                    base.DeleteByID(id)
                )
                .ContinueWith(
                    x => new OperationResult
                    {
                        IsSuccessful = x.Result.All(r => r.IsSuccessful),
                        Reason = x.Result.SelectMany(r => r.FlattenReasons()).FirstOrDefault(),
                        Comments = x.Result.SelectMany(r => r.FlattenReasons()).ToArray().Jump(1),
                    }
                );
        }

        public override Task<OperationResult<TId>[]> DeleteByIDs(params TId[] ids)
        {
            return
                Task.WhenAll(
                    ids.Select(
                        id => DeleteByID(id).ContinueWith(x => x.Result.WithPayload(id))
                    )
                );
        }

        public override Task<OperationResult> Save(TEntity entity)
        {
            return
                Task.WhenAll(
                    inMemoryStorage.Save(entity),
                    base.Save(entity)
                )
                .ContinueWith(
                    x => new OperationResult
                    {
                        IsSuccessful = x.Result.All(r => r.IsSuccessful),
                        Reason = x.Result.SelectMany(r => r.FlattenReasons()).FirstOrDefault(),
                        Comments = x.Result.SelectMany(r => r.FlattenReasons()).ToArray().Jump(1),
                    }
                );
        }

        public override async Task<OperationResult<IDisposableEnumerable<TEntity>>> StreamAll()
        {
            if (!entityStorageFolder.Exists)
                return OperationResult.Win("There are no entities of this type, nothing to load").WithPayload(new DataStream<TEntity>(new TEntity[0]) as IDisposableEnumerable<TEntity>);

            IEnumerable<FileInfo> allFiles = entityStorageFolder.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly);
            using (IDisposableEnumerable<TEntity> inMemoryEntries = (await inMemoryStorage.StreamAll()).Payload)
            {
                TId[] inMemoryIDs = inMemoryEntries.Select(i => i.ID).ToArray();

                FileInfo[] entriesNotInMemory = allFiles.Where(file => Path.GetFileNameWithoutExtension(file.Name).ToLowerInvariant().NotIn(inMemoryIDs.Select(x => x.ToString().ToLowerInvariant()))).ToArray();
                IEnumerable<TEntity> allEntitiesEnumeration
                    = inMemoryIDs
                    .Select(id => inMemoryStorage.LoadByID(id).GetAwaiter().GetResult().Payload)
                    .Concat(
                        entriesNotInMemory
                        .Select(file =>
                        {
                            OperationResult<TEntity> entityResult = LoadEntityFromFile(file).GetAwaiter().GetResult();
                            if (!entityResult.IsSuccessful || entityResult.Payload == null)
                                return entityResult.Payload;
                            inMemoryStorage.Save(entityResult.Payload).Wait();
                            return entityResult.Payload;
                        })
                    )
                    .Where(x => x != null);

                return OperationResult.Win().WithPayload(new DataStream<TEntity>(allEntitiesEnumeration) as IDisposableEnumerable<TEntity>);
            }
        }

        public override async Task<OperationResult<IDisposableEnumerable<TEntity>>> Stream(TFilter filter)
        {
            OperationResult<IDisposableEnumerable<TEntity>> allStream = await StreamAll();
            using (IDisposableEnumerable<TEntity> all = allStream.Payload)
            {
                IDisposableEnumerable<TEntity> payload = new DataStream<TEntity>(ApplyFilter(all, filter));
                return OperationResult.Win().WithPayload(payload);
            }
        }

        public override async Task<OperationResult<Page<TEntity>>> LoadPage(TFilter filter)
        {
            OperationResult<IDisposableEnumerable<TEntity>> filteredStream = await Stream(filter);
            using (IDisposableEnumerable<TEntity> filtered = filteredStream.Payload)
            {
                return OperationResult.Win().WithPayload(Page<TEntity>.For(filter, entityStorageFolder.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly).Count(), filtered.ToArray()));
            }
        }
    }
}
