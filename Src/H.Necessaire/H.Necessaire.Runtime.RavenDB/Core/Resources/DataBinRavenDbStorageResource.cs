using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Operations.Attachments;
using Raven.Client.Documents.Session;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.RavenDB.Core.Resources
{
    internal class DataBinRavenDbStorageResource
        : RavenDbStorageServiceBase<Guid, DataBinMeta, DataBinFilter, DataBinRavenDbStorageResource.DataBinMetaFilterIndex>
        , ImAStorageService<Guid, DataBin>
        , ImAStorageBrowserService<DataBin, DataBinFilter>
    {
        ImALogger logger;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            logger = dependencyProvider.GetLogger<DataBinRavenDbStorageResource>();
        }

        protected override IAsyncDocumentQuery<DataBinMeta> ApplyFilter(IAsyncDocumentQuery<DataBinMeta> result, DataBinFilter filter)
        {
            if (filter?.IDs?.Any() ?? false)
            {
                result = result.WhereIn(nameof(DataBinMeta.ID), filter.IDs.ToStringArray());
            }

            if (filter?.Names?.Any(x => !string.IsNullOrWhiteSpace(x)) ?? false)
            {
                result = result.WhereIn(nameof(DataBinMeta.Name), filter.Names.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray());
            }

            if (filter?.FormatIDs?.Any() == true)
            {
                result = result.WhereIn("FormatID", filter.FormatIDs);
            }

            if (filter?.FormatExtensions?.Any() == true)
            {
                result = result.WhereIn("FormatExtension", filter.FormatExtensions);
            }

            if (filter?.FormatMimeTypes?.Any() == true)
            {
                result = result.WhereIn("FormatMimeType", filter.FormatMimeTypes);
            }

            if (filter?.FormatEncodings?.Any() == true)
            {
                result = result.WhereIn("FormatEncoding", filter.FormatEncodings);
            }

            if (filter?.FromInclusive != null)
            {
                result = result.WhereGreaterThanOrEqual(nameof(DataBinMeta.AsOf), filter.FromInclusive.Value);
            }

            if (filter?.ToInclusive != null)
            {
                result = result.WhereLessThanOrEqual(nameof(DataBinMeta.AsOf), filter.ToInclusive.Value);
            }

            if (filter?.Notes?.Any() == true)
            {
                result = result.ContainsAny("NotesAsStrings", filter.Notes.Select(x => x.ToString()).ToArray());
            }

            return result;
        }

        protected override IDocumentQuery<DataBinMeta> ApplyFilterSync(IDocumentQuery<DataBinMeta> result, DataBinFilter filter)
        {
            if (filter?.IDs?.Any() ?? false)
            {
                result = result.WhereIn(nameof(DataBinMeta.ID), filter.IDs.ToStringArray());
            }

            if (filter?.Names?.Any(x => !string.IsNullOrWhiteSpace(x)) ?? false)
            {
                result = result.WhereIn(nameof(DataBinMeta.Name), filter.Names.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray());
            }

            if (filter?.FormatIDs?.Any() == true)
            {
                result = result.WhereIn("FormatID", filter.FormatIDs);
            }

            if (filter?.FormatExtensions?.Any() == true)
            {
                result = result.WhereIn("FormatExtension", filter.FormatExtensions);
            }

            if (filter?.FormatMimeTypes?.Any() == true)
            {
                result = result.WhereIn("FormatMimeType", filter.FormatMimeTypes);
            }

            if (filter?.FormatEncodings?.Any() == true)
            {
                result = result.WhereIn("FormatEncoding", filter.FormatEncodings);
            }

            if (filter?.FromInclusive != null)
            {
                result = result.WhereGreaterThanOrEqual(nameof(DataBinMeta.AsOf), filter.FromInclusive.Value);
            }

            if (filter?.ToInclusive != null)
            {
                result = result.WhereLessThanOrEqual(nameof(DataBinMeta.AsOf), filter.ToInclusive.Value);
            }

            if (filter?.Notes?.Any() == true)
            {
                result = result.ContainsAny("NotesAsStrings", filter.Notes.Select(x => x.ToString()).ToArray());
            }

            return result;
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

                    using (ImADataBinStream dataBinSourceStream = await entity.OpenDataBinStream())
                    using (IAsyncDocumentSession dbSession = NewWriteSession())
                    {
                        dbSession.Advanced.Attachments.Store(entity.ID.ToString(), entity.GenerateUniqueFileName(), dataBinSourceStream.DataStream, entity.Format?.MimeType);

                        await dbSession.SaveChangesAsync();
                    }

                    result = OperationResult.Win();
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        string message = $"Error occurred while trying to save DataBin ({entity.ID}) to RavenDB";
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
                        string message = $"Error occurred while trying to load DataBin ({id}) from RavenDB";
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
                        string message = $"Error occurred while trying to load {ids?.Length ?? 0} DataBins from RavenDB";
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
                        string message = $"Error occurred while trying to filter and load DataBins page from RavenDB";
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
                        string message = $"Error occurred while trying to filter and stream DataBins from RavenDB";
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
                        string message = $"Error occurred while trying to stream DataBins from RavenDB";
                        await logger.LogError($"{message} ({ex.Message})", ex);
                        result = OperationResult.Fail(ex, message).WithoutPayload<IDisposableEnumerable<DataBin>>();
                    }
                );

            return result;
        }

        private async Task<ImADataBinStream> OpenDataBinContentStream(DataBinMeta meta)
        {
            ImADataBinStream result = null;

            IAsyncDocumentSession dbSession = NewReadSession();

            await
                new Func<Task>(async () =>
                {
                    AttachmentResult attachment = await dbSession.Advanced.Attachments.GetAsync(meta.ID.ToString(), meta.GenerateUniqueFileName());

                    result = attachment.Stream.ToDataBinStream(attachment, dbSession);

                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        dbSession.Dispose();
                        dbSession = null;
                        string message = $"Error occurred while trying to open DataBin content stream for {meta.ID} from RavenDB";
                        await logger.LogError($"{message} ({ex.Message})", ex, meta);
                        result = null;
                    }
                );

            return result;
        }

        public class DataBinMetaFilterIndex : AbstractIndexCreationTask<DataBinMeta>
        {
            public DataBinMetaFilterIndex()
            {
                Map = docs => docs.Select(doc =>
                    new
                    {
                        doc.ID,
                        doc.Name,
                        doc.AsOf,
                        FormatID = doc.Format.ID,
                        FormatExtension = doc.Format.Extension,
                        FormatMimeType = doc.Format.MimeType,
                        FormatEncoding = doc.Format.Encoding,
                        NotesAsStrings = doc.Notes.Select(x => x.ToString()).ToArray(),
                    }
                );
            }
        }
    }
}
