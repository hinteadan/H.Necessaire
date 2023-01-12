using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Resources.Concrete
{
    internal class DataBinFileSystemStorageResource : JsonCachedFileSystemStorageServiceBase<Guid, DataBin, DataBinFilter>
    {
        ImALogger logger;
        DataBinMetaFileSystemStorageResource dataBinMetaFileSystemStorageResource = null;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            logger = dependencyProvider.GetLogger<DataBinFileSystemStorageResource>();
            dataBinMetaFileSystemStorageResource = dependencyProvider.Get<DataBinMetaFileSystemStorageResource>();
        }

        public override async Task<OperationResult> Save(DataBin entity)
        {
            OperationResult result = OperationResult.Fail("Not yet started");

            await
                new Func<Task>(async () =>
                {
                    result = await EnsureEntityStorageFolder();

                    if (!result.IsSuccessful)
                        return;

                    result = await dataBinMetaFileSystemStorageResource.Save(entity.ToMeta());

                    if (!result.IsSuccessful)
                        return;

                    FileInfo dataFile = BuildDataFile(entity.ID, entity.Format?.Extension);

                    using (FileStream dataFileStream = dataFile.Create())
                    using (ImADataBinStream sourceStream = await entity.OpenDataBinStream())
                    {
                        await sourceStream.DataStream.CopyToAsync(dataFileStream);
                    }

                    result = OperationResult.Win();
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        string message = $"Error occurred while trying to save DataBin ({entity.ID}) to file system";
                        await logger.LogError($"{message} ({ex.Message})", ex, entity);
                        result = OperationResult.Fail(ex, message);
                    }
                );

            return result;
        }

        public override async Task<OperationResult> DeleteByID(Guid id)
        {
            OperationResult result = OperationResult.Fail("Not yet started");

            await
                new Func<Task>(async () =>
                {
                    result = await dataBinMetaFileSystemStorageResource.DeleteByID(id);

                    if (!result.IsSuccessful)
                        return;

                    FileInfo dataFile
                        = entityStorageFolder
                        .EnumerateFiles()
                        .SingleOrDefault(
                            f => f.Name.StartsWith(id.ToString(), StringComparison.InvariantCultureIgnoreCase)
                        );

                    if (dataFile != null)
                    {
                        dataFile.Delete();
                    }

                    result = OperationResult.Win();
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        string message = $"Error occurred while trying to delete DataBin ({id}) data file";
                        await logger.LogError($"{message} ({ex.Message})", ex, id);
                        result = OperationResult.Fail(ex, message);
                    }
                );

            return result;
        }

        public override async Task<OperationResult<DataBin>> LoadByID(Guid id)
        {
            OperationResult<DataBin> result = OperationResult.Fail("Not yet started").WithoutPayload<DataBin>();

            await
                new Func<Task>(async () =>
                {
                    OperationResult<DataBinMeta> metaLoadResult = await dataBinMetaFileSystemStorageResource.LoadByID(id);

                    if (!metaLoadResult.IsSuccessful)
                    {
                        result = metaLoadResult.WithoutPayload<DataBin>();
                        return;
                    }

                    DataBinMeta meta = metaLoadResult.Payload;

                    DataBin dataBin = meta.ToBin(OpenDataBinContentStream);

                    result = dataBin.ToWinResult();
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        string message = $"Error occurred while trying to load DataBin ({id})";
                        await logger.LogError($"{message} ({ex.Message})", ex, id);
                        result = OperationResult.Fail(ex, message).WithoutPayload<DataBin>();
                    }
                );

            return result;
        }

        public override async Task<OperationResult<IDisposableEnumerable<DataBin>>> StreamAll()
        {
            OperationResult<IDisposableEnumerable<DataBin>> result = OperationResult.Fail("Not yet started").WithoutPayload<IDisposableEnumerable<DataBin>>();

            await
                new Func<Task>(async () =>
                {
                    OperationResult<IDisposableEnumerable<DataBinMeta>> metaStreamResult
                        = await dataBinMetaFileSystemStorageResource.StreamAll();

                    if (!metaStreamResult.IsSuccessful)
                    {
                        result = metaStreamResult.WithoutPayload<IDisposableEnumerable<DataBin>>();
                        return;
                    }

                    IDisposableEnumerable<DataBin> dataBinStream
                        = metaStreamResult.Payload.ProjectTo(x => x.ToBin(OpenDataBinContentStream));

                    result = dataBinStream.ToWinResult();
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        string message = $"Error occurred while trying to stream DataBins from file system";
                        await logger.LogError($"{message} ({ex.Message})", ex);
                        result = OperationResult.Fail(ex, message).WithoutPayload<IDisposableEnumerable<DataBin>>();
                    }
                );

            return result;
        }

        protected override IEnumerable<DataBin> ApplyFilter(IEnumerable<DataBin> stream, DataBinFilter filter)
        {
            return stream.ApplyFilter(filter);
        }

        private async Task<ImADataBinStream> OpenDataBinContentStream(DataBinMeta meta)
        {
            FileInfo dataFile = BuildDataFile(meta.ID, meta.Format?.Extension);
            if (!dataFile.Exists)
            {
                await logger.LogWarn($"Trying to open a DataBin ({meta.ID}) data file that doesn't exist: {dataFile.FullName}");
                return null;
            }

            return dataFile.OpenRead().ToDataBinStream();
        }

        private FileInfo BuildDataFile(Guid dataBinID, string extension)
        {
            return new FileInfo(Path.Combine(entityStorageFolder.FullName, $"{dataBinID}.{extension}"));
        }
    }

    internal class DataBinMetaFileSystemStorageResource : JsonCachedFileSystemStorageServiceBase<Guid, DataBinMeta, DataBinFilter>
    {
        protected override IEnumerable<DataBinMeta> ApplyFilter(IEnumerable<DataBinMeta> stream, DataBinFilter filter)
        {
            return stream.ApplyFilter(filter);
        }
    }

    internal static class DataBinMetaExtensions
    {
        public static IEnumerable<T> ApplyFilter<T>(this IEnumerable<T> stream, DataBinFilter filter)
            where T : DataBinMeta
        {
            IEnumerable<T> result = stream;

            if (filter?.IDs?.Any() ?? false)
            {
                result = result.Where(x => x.ID.In(filter.IDs));
            }

            if (filter?.Names?.Any(x => !string.IsNullOrWhiteSpace(x)) ?? false)
            {
                result = result.Where(x => x.Name.In(filter.Names.Where(n => !string.IsNullOrWhiteSpace(n)), (name, filterName) => name?.IndexOf(filterName, StringComparison.InvariantCultureIgnoreCase) >= 0));
            }

            if (filter?.FormatIDs?.Any() == true)
            {
                result = result.Where(x => x.Format != null && x.Format.ID.In(filter.FormatIDs, (a, b) => string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase)));
            }

            if (filter?.FormatExtensions?.Any() == true)
            {
                result = result.Where(x => x.Format != null && x.Format.Extension.In(filter.FormatExtensions, (a, b) => string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase)));
            }

            if (filter?.FormatMimeTypes?.Any() == true)
            {
                result = result.Where(x => x.Format != null && x.Format.MimeType.In(filter.FormatMimeTypes, (a, b) => string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase)));
            }

            if (filter?.FormatEncodings?.Any() == true)
            {
                result = result.Where(x => x.Format != null && x.Format.Encoding.In(filter.FormatEncodings, (a, b) => string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase)));
            }

            if (filter?.Notes?.Any() == true)
            {
                result = result.Where(x => x.Notes?.Any(noteToCheck =>
                    filter.Notes.Any(
                        filterNote =>
                            string.Equals(noteToCheck.ID, filterNote.ID, StringComparison.InvariantCultureIgnoreCase)
                            &&
                            string.Equals(noteToCheck.Value, filterNote.Value, StringComparison.InvariantCultureIgnoreCase)
                )) == true);
            }

            if (filter?.FromInclusive != null)
            {
                result = result.Where(x => x.AsOf >= filter.FromInclusive.Value);
            }

            if (filter?.ToInclusive != null)
            {
                result = result.Where(x => x.AsOf <= filter.ToInclusive.Value);
            }

            result = result.ApplySortAndPageFilterIfAny(filter);

            return result;
        }
    }
}
