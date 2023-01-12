using Dapper;
using H.Necessaire.Dapper;
using H.Necessaire.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.SqlServer.Core.Resources
{
    internal partial class DataBinSqlServerStorageResource
        : DapperStorageServiceBase<Guid, DataBinMeta, DataBinSqlServerStorageResource.DataBinSqlMetaEntry, DataBinFilter>
        , ImAStorageService<Guid, DataBin>
        , ImAStorageBrowserService<DataBin, DataBinFilter>
    {
        #region Construct
        public DataBinSqlServerStorageResource() : base(connectionString: null, tableName: dataBinTableName, databaseName: "H.Necessaire.Core") { }
        protected override Task<SqlMigration[]> GetAllMigrations() => sqlMigrations.AsTask();
        ImALogger logger;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            logger = dependencyProvider.GetLogger<DataBinSqlServerStorageResource>();
        }
        #endregion

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

        public async Task<OperationResult> Save(DataBin entity)
        {
            OperationResult result = OperationResult.Fail("Not yet started");

            await
                new Func<Task>(async () =>
                {
                    result = await (this as ImAStorageService<Guid, DataBinMeta>).Save(entity.ToMeta());
                    if (!result.IsSuccessful)
                        return;

                    using (SqlConnection dbConnection = new SqlConnection(connectionString))
                    {
                        await dbConnection.OpenAsync();

                        using (ImADataBinStream dataBinSourceStream = await entity.OpenDataBinStream())
                        using (SqlCommand sqlCommand = new SqlCommand($"UPDATE [{dataBinTableName}] SET [Content] = @contentStream WHERE [{nameof(DataBinSqlMetaEntry.ID)}]=@{nameof(entity.ID)}", dbConnection))
                        {
                            sqlCommand.Parameters.AddWithValue(nameof(entity.ID), entity.ID);
                            sqlCommand.Parameters.Add("@contentStream", SqlDbType.Binary, -1).Value = dataBinSourceStream.DataStream;
                            await sqlCommand.ExecuteNonQueryAsync();
                        }
                    }

                    result = OperationResult.Win();
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        string message = $"Error occurred while trying to save DataBin ({entity.ID}) to SQL Server";
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
                        string message = $"Error occurred while trying to load DataBin ({id}) from SQL Server";
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
                        string message = $"Error occurred while trying to load {ids?.Length ?? 0} DataBins from SQL Server";
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
                        string message = $"Error occurred while trying to filter and load DataBins page from SQL Server";
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
                        string message = $"Error occurred while trying to filter and stream DataBins from SQL Server";
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
                        string message = $"Error occurred while trying to stream DataBins from SQL Server";
                        await logger.LogError($"{message} ({ex.Message})", ex);
                        result = OperationResult.Fail(ex, message).WithoutPayload<IDisposableEnumerable<DataBin>>();
                    }
                );

            return result;
        }

        private async Task<ImADataBinStream> OpenDataBinContentStream(DataBinMeta meta)
        {
            ImADataBinStream result = null;

            SqlConnection dbConnection = null;
            SqlCommand sqlCommand = null;
            SqlDataReader sqlDataReader = null;

            await
                new Func<Task>(async () =>
                {
                    dbConnection = new SqlConnection(connectionString);
                    await dbConnection.OpenAsync();
                    sqlCommand = new SqlCommand($"SELECT [Content] FROM [{dataBinTableName}] WHERE [{nameof(DataBinSqlMetaEntry.ID)}]=@{nameof(meta.ID)}", dbConnection);
                    sqlCommand.Parameters.AddWithValue(nameof(meta.ID), meta.ID);
                    sqlDataReader = await sqlCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

                    if (!(await sqlDataReader.ReadAsync()))
                    {
                        new Action(() =>
                        {
                            if (sqlDataReader != null)
                            {
                                sqlDataReader.Dispose();
                                sqlDataReader = null;
                            }
                        }).TryOrFailWithGrace();
                        new Action(() =>
                        {
                            if (sqlCommand != null)
                            {
                                sqlCommand.Dispose();
                                sqlCommand = null;
                            }
                        }).TryOrFailWithGrace();
                        new Action(() =>
                        {
                            if (dbConnection != null)
                            {
                                dbConnection.Dispose();
                                dbConnection = null;
                            }
                        }).TryOrFailWithGrace();
                        await logger.LogWarn($"Cannot read DataBin {meta.ID} content stream from SQL Server because ReadAsync() is FALSE", meta);
                        return;
                    }

                    if (await sqlDataReader.IsDBNullAsync(0))
                    {
                        new Action(() =>
                        {
                            if (sqlDataReader != null)
                            {
                                sqlDataReader.Dispose();
                                sqlDataReader = null;
                            }
                        }).TryOrFailWithGrace();
                        new Action(() =>
                        {
                            if (sqlCommand != null)
                            {
                                sqlCommand.Dispose();
                                sqlCommand = null;
                            }
                        }).TryOrFailWithGrace();
                        new Action(() =>
                        {
                            if (dbConnection != null)
                            {
                                dbConnection.Dispose();
                                dbConnection = null;
                            }
                        }).TryOrFailWithGrace();
                        await logger.LogWarn($"Cannot read DataBin {meta.ID} content stream from SQL Server because it's DB NULL", meta);
                        return;
                    }

                    Stream sqlContentStream = sqlDataReader.GetStream(0);

                    result = sqlContentStream.ToDataBinStream(sqlDataReader, sqlCommand, dbConnection);
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        new Action(() =>
                        {
                            if (dbConnection != null)
                            {
                                dbConnection.Dispose();
                                dbConnection = null;
                            }
                        }).TryOrFailWithGrace();

                        new Action(() =>
                        {
                            if (sqlCommand != null)
                            {
                                sqlCommand.Dispose();
                                sqlCommand = null;
                            }
                        }).TryOrFailWithGrace();

                        new Action(() =>
                        {
                            if (sqlDataReader != null)
                            {
                                sqlDataReader.Dispose();
                                sqlDataReader = null;
                            }
                        }).TryOrFailWithGrace();

                        string message = $"Error occurred while trying to open DataBin content stream for {meta.ID} from SQL Server";
                        await logger.LogError($"{message} ({ex.Message})", ex, meta);
                        result = null;
                    }
                );

            return result;
        }

        public class DataBinSqlMetaEntry : SqlEntryBase
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
                        x.AsOfTicks = entity.AsOf.Ticks;
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
                        x.AsOf = new DateTime(sqlEntity.AsOfTicks);
                        x.Format = sqlEntity.FormatJson?.JsonToObject<DataBinFormatInfo>();
                        x.Notes = sqlEntity.NotesJson?.DeserializeToNotes();
                    })
                    ;
            }
        }
    }
}
