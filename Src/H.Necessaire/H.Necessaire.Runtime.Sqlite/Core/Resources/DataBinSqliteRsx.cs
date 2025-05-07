using Dapper;
using H.Necessaire.Dapper;
using H.Necessaire.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Sqlite.Core.Resources
{
    internal class DataBinSqliteRsx
        : DapperSqliteStorageResourceBase<
            Guid,
            DataBinMeta,
            DataBinSqliteRsx.DataBinSqlMetaEntry,
            DataBinFilter
        >
        , ImAStorageService<Guid, DataBin>
        , ImAStorageBrowserService<DataBin, DataBinFilter>
    {
        #region Construct
        static SqlMigration[] sqlMigrations = null;
        DirectoryInfo binDataFolder;
        public DataBinSqliteRsx() : base(connectionString: null, tableName: "H.Necessaire.DataBin", databaseName: "H.Necessaire.Core") { }
        protected override async Task<SqlMigration[]> GetAllMigrations()
        {
            if (sqlMigrations != null)
                return sqlMigrations;

            sqlMigrations = new SqlMigration[] {
                await new SqlMigration{
                    ResourceIdentifier = nameof(DataBin),
                    VersionNumber = new VersionNumber(1, 0),
                }.AndAsync(async x => x.SqlCommand = await ReadMigrationSqlCommand(x, typeof(SqliteDependencyGroup).Assembly)),
            };

            return sqlMigrations;
        }
        ImALogger logger;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            logger = dependencyProvider.GetLogger<DataBinSqliteRsx>();
            binDataFolder = new DirectoryInfo(Path.Combine(dependencyProvider.GetRuntimeConfig().Get("AppDataFolderPath").ToString(), "BinData")).And(x => x.Create());
            ;
        }
        #endregion

        public async Task<OperationResult> Save(DataBin entity)
        {
            OperationResult result = OperationResult.Fail("Not yet started");

            await
                new Func<Task>(async () =>
                {
                    result = await (this as ImAStorageService<Guid, DataBinMeta>).Save(entity);
                    if (!result.IsSuccessful)
                        return;

                    FileInfo dataFileInfo = GetDataFileInfo(entity);
                    if (dataFileInfo.Exists)
                        dataFileInfo.Delete();

                    using (ImADataBinStream dataBinStream = await entity.OpenDataBinStream())
                    {
                        if (dataBinStream?.DataStream is null)
                        {
                            result = OperationResult.Fail($"DataBin {entity.ID} data stream is NULL");
                            return;
                        }

                        using (Stream dataStream = dataBinStream.DataStream)
                        using (FileStream fileStream = dataFileInfo.OpenWrite())
                        {
                            dataStream.CopyTo(fileStream);
                        }
                    }

                    result = OperationResult.Win();
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        string message = $"Error occurred while trying to save DataBin ({entity.ID}) to SQLite";
                        await logger.LogError($"{message} ({ex.Message})", ex, entity);
                        result = OperationResult.Fail(ex, message);
                    }
                );

            return result;
        }

        async Task<OperationResult<DataBin>> ImAStorageService<Guid, DataBin>.LoadByID(Guid id)
        {
            OperationResult<DataBin> result = OperationResult.Fail("Not yet started").WithoutPayload<DataBin>();

            await
                new Func<Task>(async () =>
                {
                    OperationResult<DataBinMeta> metaLoadResult
                        = await (this as ImAStorageService<Guid, DataBinMeta>).LoadByID(id);

                    if (!metaLoadResult.IsSuccessful)
                    {
                        result = metaLoadResult.WithoutPayload<DataBin>();
                        return;
                    }

                    DataBin dataBin = metaLoadResult.Payload.ToBin(OpenDataBinContentStream);

                    result = dataBin.ToWinResult();
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        string message = $"Error occurred while trying to load DataBin ({id}) from SQLite";
                        await logger.LogError($"{message} ({ex.Message})", ex, id);
                        result = OperationResult.Fail(ex, message).WithoutPayload<DataBin>();
                    }
                );

            return result;
        }

        async Task<OperationResult<DataBin>[]> ImAStorageService<Guid, DataBin>.LoadByIDs(params Guid[] ids)
        {
            OperationResult<DataBin>[] result = new OperationResult<DataBin>[0];

            await
                new Func<Task>(async () =>
                {
                    OperationResult<DataBinMeta>[] metaLoadResults
                        = await (this as ImAStorageService<Guid, DataBinMeta>).LoadByIDs(ids);

                    result
                        = metaLoadResults
                        .Select(x =>
                            !x.IsSuccessful
                            ? x.WithPayload(x.Payload?.ToBin(null))
                            : x.Payload.ToBin(OpenDataBinContentStream).ToWinResult()
                        )
                        .ToArray()
                        ;
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        string message = $"Error occurred while trying to load {ids?.Length ?? 0} DataBins from SQLite";
                        await logger.LogError($"{message} ({ex.Message})", ex, ids);
                        result = new OperationResult<DataBin>[0];
                    }
                );

            return result;
        }

        async Task<OperationResult<Page<DataBin>>> ImAStorageBrowserService<DataBin, DataBinFilter>.LoadPage(DataBinFilter filter)
        {
            OperationResult<Page<DataBin>> result = OperationResult.Fail("Not yet started").WithoutPayload<Page<DataBin>>();

            await
                new Func<Task>(async () =>
                {
                    OperationResult<Page<DataBinMeta>> metaPageResult
                        = await (this as ImAStorageBrowserService<DataBinMeta, DataBinFilter>).LoadPage(filter);

                    if (!metaPageResult.IsSuccessful)
                    {
                        result = metaPageResult.WithoutPayload<Page<DataBin>>();
                        return;
                    }

                    Page<DataBin> resultPage
                        = new Page<DataBin>
                        {
                            PageIndex = metaPageResult.Payload.PageIndex,
                            PageSize = metaPageResult.Payload.PageSize,
                            TotalNumberOfPages = metaPageResult.Payload.TotalNumberOfPages,
                            Content = metaPageResult.Payload.Content?.Select(x => x.ToBin(OpenDataBinContentStream)).ToArray(),
                        };

                    result = resultPage.ToWinResult();
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        string message = $"Error occurred while trying to filter and load DataBins page from SQLite";
                        await logger.LogError($"{message} ({ex.Message})", ex, filter);
                        result = OperationResult.Fail(ex, message).WithoutPayload<Page<DataBin>>();
                    }
                );

            return result;
        }

        async Task<OperationResult<IDisposableEnumerable<DataBin>>> ImAStorageBrowserService<DataBin, DataBinFilter>.Stream(DataBinFilter filter)
        {
            OperationResult<IDisposableEnumerable<DataBin>> result = OperationResult.Fail("Not yet started").WithoutPayload<IDisposableEnumerable<DataBin>>();

            await
                new Func<Task>(async () =>
                {
                    OperationResult<IDisposableEnumerable<DataBinMeta>> metaStreamResult
                        = await (this as ImAStorageBrowserService<DataBinMeta, DataBinFilter>).Stream(filter);

                    if (!metaStreamResult.IsSuccessful)
                    {
                        result = metaStreamResult.WithoutPayload<IDisposableEnumerable<DataBin>>();
                        return;
                    }

                    IDisposableEnumerable<DataBin> resultStream
                        = metaStreamResult
                        .Payload
                        .ProjectTo(x => x.ToBin(OpenDataBinContentStream))
                        ;

                    result = resultStream.ToWinResult();
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        string message = $"Error occurred while trying to filter and stream DataBins from SQLite";
                        await logger.LogError($"{message} ({ex.Message})", ex, filter);
                        result = OperationResult.Fail(ex, message).WithoutPayload<IDisposableEnumerable<DataBin>>();
                    }
                );

            return result;
        }

        async Task<OperationResult<IDisposableEnumerable<DataBin>>> ImAStorageBrowserService<DataBin, DataBinFilter>.StreamAll()
        {
            OperationResult<IDisposableEnumerable<DataBin>> result = OperationResult.Fail("Not yet started").WithoutPayload<IDisposableEnumerable<DataBin>>();

            await
                new Func<Task>(async () =>
                {
                    OperationResult<IDisposableEnumerable<DataBinMeta>> metaStreamResult
                        = await (this as ImAStorageBrowserService<DataBinMeta, DataBinFilter>).StreamAll();

                    if (!metaStreamResult.IsSuccessful)
                    {
                        result = metaStreamResult.WithoutPayload<IDisposableEnumerable<DataBin>>();
                        return;
                    }

                    IDisposableEnumerable<DataBin> resultStream
                        = metaStreamResult
                        .Payload
                        .ProjectTo(x => x.ToBin(OpenDataBinContentStream))
                        ;

                    result = resultStream.ToWinResult();
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        string message = $"Error occurred while trying to stream DataBins from SQLite";
                        await logger.LogError($"{message} ({ex.Message})", ex);
                        result = OperationResult.Fail(ex, message).WithoutPayload<IDisposableEnumerable<DataBin>>();
                    }
                );

            return result;
        }

        async Task<OperationResult> ImAStorageService<Guid, DataBin>.DeleteByID(Guid id)
        {
            if (!(await (this as ImAStorageService<Guid, DataBinMeta>).LoadByID(id)).Ref(out var metaLoadResult, out var dataBinMeta))
                return metaLoadResult;

            if (dataBinMeta is null)
                return OperationResult.Win($"DataBin with ID {id} doesn't exist, nothing to delete");

            FileInfo dataFileInfo = GetDataFileInfo(dataBinMeta);

            if (dataFileInfo.Exists)
            {
                if (!HSafe.Run(dataFileInfo.Delete, $"Delete DataBin file {dataFileInfo.FullName}").Ref(out var fileDeleteResult))
                    return fileDeleteResult;
            }

            if (!(await (this as ImAStorageService<Guid, DataBinMeta>).DeleteByID(id)).Ref(out var metaDeleteResult))
                return metaDeleteResult;

            return OperationResult.Win();
        }

        async Task<OperationResult<Guid>[]> ImAStorageService<Guid, DataBin>.DeleteByIDs(params Guid[] ids)
        {
            OperationResult<DataBinMeta>[] loadResults = await (this as ImAStorageService<Guid, DataBinMeta>).LoadByIDs(ids);
            if (loadResults.IsEmpty())
                return ids?.Select(x => x.ToWinResult()).ToArray();

            foreach (DataBinMeta meta in loadResults.Select(x => x.Payload))
            {
                FileInfo dataFileInfo = GetDataFileInfo(meta);
                if (dataFileInfo.Exists)
                {
                    if (!HSafe.Run(dataFileInfo.Delete, $"Delete DataBin file {dataFileInfo.FullName}").Ref(out var fileDeleteResult))
                        return fileDeleteResult.WithPayload(meta.ID).AsArray();
                }
            }

            return await (this as ImAStorageService<Guid, DataBinMeta>).DeleteByIDs(ids);
        }

        protected override ISqlFilterCriteria[] ApplyFilter(DataBinFilter filter, DynamicParameters sqlParams)
        {
            List<ISqlFilterCriteria> result = new List<ISqlFilterCriteria>();

            if (filter?.IDs?.Any() ?? false)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(DataBinSqlMetaEntry.ID), parameterName: nameof(filter.IDs), @operator: "IN"));
            }

            if (filter?.Names?.Any(x => !string.IsNullOrWhiteSpace(x)) ?? false)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(DataBinSqlMetaEntry.Name), parameterName: nameof(filter.Names), @operator: "IN"));
            }

            if (filter?.FormatIDs?.Any() == true)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(DataBinSqlMetaEntry.FormatID), parameterName: nameof(filter.FormatIDs), @operator: "IN"));
            }

            if (filter?.FormatExtensions?.Any() == true)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(DataBinSqlMetaEntry.FormatExtension), parameterName: nameof(filter.FormatExtensions), @operator: "IN"));
            }

            if (filter?.FormatMimeTypes?.Any() == true)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(DataBinSqlMetaEntry.FormatMimeType), parameterName: nameof(filter.FormatMimeTypes), @operator: "IN"));
            }

            if (filter?.FormatEncodings?.Any() == true)
            {
                result.Add(new SqlFilterCriteria(columnName: nameof(DataBinSqlMetaEntry.FormatEncoding), parameterName: nameof(filter.FormatEncodings), @operator: "IN"));
            }

            if (filter?.FromInclusive != null)
            {
                sqlParams.Add($"{nameof(filter.FromInclusive)}Ticks", filter.FromInclusive.Value.Ticks);
                result.Add(new SqlFilterCriteria(columnName: nameof(DataBinSqlMetaEntry.AsOfTicks), parameterName: $"{nameof(filter.FromInclusive)}Ticks", @operator: ">="));
            }

            if (filter?.ToInclusive != null)
            {
                sqlParams.Add($"{nameof(filter.ToInclusive)}Ticks", filter.ToInclusive.Value.Ticks);
                result.Add(new SqlFilterCriteria(columnName: nameof(DataBinSqlMetaEntry.AsOfTicks), parameterName: $"{nameof(filter.ToInclusive)}Ticks", @operator: "<="));
            }

            if (filter?.Notes?.Any() == true)
            {
                List<ISqlFilterCriteria> noteCriterias = new List<ISqlFilterCriteria>(filter.Notes.Length);
                int index = -1;
                foreach (Note note in filter.Notes)
                {
                    index++;
                    sqlParams.Add($"{nameof(filter.Notes)}{index}", note.ToString());
                    noteCriterias.Add(
                        new SqlFilterCriteria(columnName: nameof(DataBinSqlMetaEntry.NotesString), parameterName: $"{nameof(filter.Notes)}{index}", @operator: "LIKE").And(x => x.LikeOperatorMatch = SqlFilterCriteria.LikeOperatorMatchType.Anywhere)
                    );
                }
                result.Add(new ComposedSqlFilterCriteria(noteCriterias.ToArray()));
            }

            return result.ToArray();
        }

        Task<ImADataBinStream> OpenDataBinContentStream(DataBinMeta meta)
        {
            FileInfo dataFileInfo = GetDataFileInfo(meta);
            if (!dataFileInfo.Exists)
                return System.IO.Stream.Null.ToDataBinStream().AsTask();

            return dataFileInfo.OpenRead().ToDataBinStream().AsTask();
        }

        FileInfo GetDataFileInfo(DataBinMeta meta)
        {
            return new FileInfo(Path.Combine(binDataFolder.FullName, meta.GenerateUniqueFileName()));
        }

        public class DataBinSqlMetaEntry : SqlEntryBase, IGuidIdentity
        {
            public Guid ID { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public DateTime AsOf { get; set; }
            public long AsOfTicks { get; set; }

            public string FormatJson { get; set; }
            public string FormatID { get; set; }
            public string FormatExtension { get; set; }
            public string FormatMimeType { get; set; }
            public string FormatEncoding { get; set; }

            public string NotesJson { get; set; }
            public string NotesString { get; set; }
        }

        public class DataBinSqlMetaEntryMapper : SqlEntityMapperBase<DataBinMeta, DataBinSqlMetaEntry>
        {
            static DataBinSqlMetaEntryMapper() => new DataBinSqlMetaEntryMapper().RegisterMapper();

            public override DataBinSqlMetaEntry MapEntityToSql(DataBinMeta entity)
            {
                return
                    base.MapEntityToSql(entity)
                    .And(x =>
                    {
                        x.AsOfTicks = entity.AsOf.EnsureUtc().Ticks;
                        x.FormatJson = entity.Format.ToJsonObject();
                        x.FormatID = entity.Format?.ID;
                        x.FormatExtension = entity.Format?.Extension;
                        x.FormatMimeType = entity.Format?.MimeType;
                        x.FormatEncoding = entity.Format?.Encoding;
                        x.NotesJson = entity.Notes.ToJsonArray();
                        x.NotesString = entity.Notes?.Any() != true ? null : string.Join(string.Empty, entity.Notes?.Select(n => n.ToString()));
                    })
                    ;
            }

            public override DataBinMeta MapSqlToEntity(DataBinSqlMetaEntry sqlEntity)
            {
                return
                    base.MapSqlToEntity(sqlEntity)
                    .And(x =>
                    {
                        x.AsOf = new DateTime(sqlEntity.AsOfTicks, DateTimeKind.Utc);
                        x.Format = sqlEntity.FormatJson?.JsonToObject<DataBinFormatInfo>();
                        x.Notes = sqlEntity.NotesJson?.DeserializeToNotes();
                    })
                    ;
            }
        }
    }
}
