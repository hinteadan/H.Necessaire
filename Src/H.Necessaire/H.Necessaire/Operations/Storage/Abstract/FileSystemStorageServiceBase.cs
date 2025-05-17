using H.Necessaire.Operations.Concrete;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public abstract class FileSystemStorageServiceBase<TId, TEntity, TFilter>
        : ImAStorageService<TId, TEntity>
        , ImAStorageBrowserService<TEntity, TFilter>
        , ImADependency
        where TFilter : IPageFilter, ISortFilter
        where TEntity : IDentityType<TId>
    {
        #region Construct
        const string rootStorageFolderName = "FileSystemStorage";
        public const string ConfigKeyRootFolderPath = "FileSystemStorageRootFolder";

        protected DirectoryInfo entityStorageFolder;
        readonly string fileExtension = "json";
        public FileSystemStorageServiceBase(DirectoryInfo rootFolder = null, string fileExtension = "json")
        {
            DirectoryInfo root = rootFolder ?? GetRootFolderFromStartAssembly();
            this.entityStorageFolder = new DirectoryInfo(Path.Combine(root.FullName, typeof(TEntity).TypeName().ToSafeFileName()));
            this.fileExtension = (fileExtension?.StartsWith(".") ?? false ? fileExtension.Substring(1) : fileExtension) ?? string.Empty;
        }

        public virtual void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            string entityStorageFolderPathFromConfig = dependencyProvider.GetRuntimeConfig()?.Get(ConfigKeyRootFolderPath)?.ToString();
            if (!string.IsNullOrWhiteSpace(entityStorageFolderPathFromConfig))
            {
                this.entityStorageFolder = new DirectoryInfo(Path.Combine(entityStorageFolderPathFromConfig, typeof(TEntity).TypeName().ToSafeFileName()));
            }
        }
        #endregion

        protected abstract Task<TEntity> ReadAndParseEntityFromStream(Stream serializedEntityStream);
        protected abstract Task SerializeEntityToStream(TEntity entityToSerialize, Stream entitySerializationStream);
        protected abstract IEnumerable<TEntity> ApplyFilter(IEnumerable<TEntity> stream, TFilter filter);

        public virtual async Task<OperationResult> Save(TEntity entity)
        {
            await EnsureEntityStorageFolder();

            FileInfo entityFile = BuildEntityFile(entity.ID);

            OperationResult result = OperationResult.Win();

            await
                new Func<Task>(async () =>
                {
                    using (Stream fileStream = entityFile.OpenWrite())
                    {
                        await SerializeEntityToStream(entity, fileStream);
                    }
                })
                .TryOrFailWithGrace(onFail: ex => result = OperationResult.Fail(ex));

            return result;

        }

        public virtual async Task<OperationResult<TEntity>> LoadByID(TId id)
        {
            if (!entityStorageFolder.Exists)
                return OperationResult.Win("There are no entities of this type, nothing to load").WithoutPayload<TEntity>();

            FileInfo entityFile = BuildEntityFile(id);

            return await LoadEntityFromFile(entityFile);
        }

        public virtual async Task<OperationResult<TEntity>[]> LoadByIDs(params TId[] ids)
        {
            if (!entityStorageFolder.Exists)
                return new OperationResult<TEntity>[0];

            return
                await
                    Task.WhenAll(
                        ids.Select(
                            id => LoadByID(id)
                        )
                    );
        }

        public virtual Task<OperationResult> DeleteByID(TId id)
        {
            if (!entityStorageFolder.Exists)
                return OperationResult.Win("There are no entities of this type, nothing to delete").AsTask();

            FileInfo entityFile = BuildEntityFile(id);

            if (!entityFile.Exists)
                return OperationResult.Win("The entity doesn't exist").AsTask();

            OperationResult result = OperationResult.Win();

            new Action(entityFile.Delete).TryOrFailWithGrace(onFail: ex => result = OperationResult.Fail(ex));

            return result.AsTask();
        }

        public virtual async Task<OperationResult<TId>[]> DeleteByIDs(params TId[] ids)
        {
            return
                await
                    Task.WhenAll(
                        ids.Select(
                            id => DeleteByID(id).ContinueWith(x => x.Result.WithPayload(id))
                        )
                    );
        }

        public virtual Task<OperationResult<IDisposableEnumerable<TEntity>>> StreamAll()
        {
            if (!entityStorageFolder.Exists)
                return OperationResult.Win("There are no entities of this type, nothing to load").WithPayload(new DataStream<TEntity>(new TEntity[0]) as IDisposableEnumerable<TEntity>).AsTask();

            IEnumerable<FileInfo> allFiles = entityStorageFolder.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly);

            IEnumerable<TEntity> allEntitiesEnumeration
                = allFiles
                .Select(entityFile => LoadEntityFromFile(entityFile).GetAwaiter().GetResult())
                .Where(x => x?.IsSuccessful ?? false)
                .Select(x => x.Payload);

            return OperationResult.Win().WithPayload(new DataStream<TEntity>(allEntitiesEnumeration) as IDisposableEnumerable<TEntity>).AsTask();
        }

        public virtual async Task<OperationResult<IDisposableEnumerable<TEntity>>> Stream(TFilter filter)
        {
            OperationResult<IDisposableEnumerable<TEntity>> allStream = await StreamAll();
            using (IDisposableEnumerable<TEntity> all = allStream.Payload)
            {
                IDisposableEnumerable<TEntity> payload = new DataStream<TEntity>(ApplyFilter(all, filter));
                return OperationResult.Win().WithPayload(payload);
            }
        }

        public virtual async Task<OperationResult<Page<TEntity>>> LoadPage(TFilter filter)
        {
            OperationResult<IDisposableEnumerable<TEntity>> filteredStream = await Stream(filter);
            using (IDisposableEnumerable<TEntity> filtered = filteredStream.Payload)
            {
                return OperationResult.Win().WithPayload(Page<TEntity>.For(filter, entityStorageFolder.Exists ? entityStorageFolder.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly).Count() : 0, filtered.ToArray()));
            }
        }

        protected Task<OperationResult> EnsureEntityStorageFolder()
        {
            if (entityStorageFolder.Exists)
                return OperationResult.Win().AsTask();

            OperationResult result = OperationResult.Win();

            new Action(entityStorageFolder.Create).TryOrFailWithGrace(onFail: ex => result = OperationResult.Fail(ex));

            return result.AsTask();
        }

        private FileInfo BuildEntityFile(TId entityID)
        {
            return new FileInfo(Path.Combine(entityStorageFolder.FullName, $"{entityID}{(string.IsNullOrWhiteSpace(fileExtension) ? string.Empty : $".{fileExtension}")}".ToSafeFileName()));
        }

        protected async Task<OperationResult<TEntity>> LoadEntityFromFile(FileInfo entityFile)
        {
            if (!entityFile.Exists)
                return OperationResult.Win("The entity doesn't exist").WithoutPayload<TEntity>();

            OperationResult<TEntity> result = OperationResult.Win().WithoutPayload<TEntity>();

            await
                new Func<Task>(async () =>
                {
                    using (Stream fileStream = entityFile.OpenRead())
                    {
                        result = OperationResult.Win().WithPayload(await ReadAndParseEntityFromStream(fileStream));
                    }
                })
                .TryOrFailWithGrace(onFail: ex => result = OperationResult.Fail(ex).WithoutPayload<TEntity>());

            return result;
        }

        private static DirectoryInfo GetRootFolderFromStartAssembly()
        {
            string path = Assembly.GetEntryAssembly()?.CodeBase;
            if (path.IsEmpty())
                return new DirectoryInfo(Directory.GetCurrentDirectory());
            string folderName = Path.GetDirectoryName(path);
            return new DirectoryInfo(Path.Combine(folderName, rootStorageFolderName));
        }
    }
}
