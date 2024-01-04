using H.Necessaire.Runtime.Google.FirestoreDB.Core.Model;
using H.Necessaire.Runtime.Google.FirestoreDB.Core.Resources.Abstract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Core.Resources
{
    internal class DataBinGoogleFirestoreDbStorageResource
        : GoogleFirestoreDbStorageResourceBase<Guid, DataBinMeta, DataBinFilter>
        , ImAStorageService<Guid, DataBin>
        , ImAStorageBrowserService<DataBin, DataBinFilter>
    {
        #region Construct
        const int maxDocumentSizeInBytes = 1 * 1024 * 1024; //1MiB: https://cloud.google.com/firestore/quotas
        const int sizeReservedForMetaInBytes = 128 * 1024;
        const int remainingSizeForDataInBytes = maxDocumentSizeInBytes - sizeReservedForMetaInBytes;
        const int maxDataSizeInBytes = remainingSizeForDataInBytes * 3 / 4;//To accomodate max Base64 string encoding length
        ImALogger logger;
        HsFirestoreStorageService dataBinPayloadStorageService;
        const string dataBinPayloadCollectionName = "DataBinPayload";
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            logger = dependencyProvider.GetLogger<DataBinGoogleFirestoreDbStorageResource>();
            dataBinPayloadStorageService = dependencyProvider.Get<HsFirestoreStorageService>().WithConfig(_ => _.And(cfg =>
            {
                cfg.CollectionName = dataBinPayloadCollectionName;
            }));
        }

        protected override IDictionary<string, Note> FilterToStoreMapping
            => new Dictionary<string, Note>() {
                { nameof(DataBinFilter.FromInclusive), nameof(DataBinMeta.AsOf).NoteAs(">=") },
                { nameof(DataBinFilter.ToInclusive), nameof(DataBinMeta.AsOf).NoteAs("<=") },
            };
        #endregion

        public async Task<OperationResult> Save(DataBin entity)
        {
            if (entity == null)
                return OperationResult.Fail("Entity is NULL");

            ImADataBinStream dataBinStream = await entity.OpenDataBinStream();
            if (dataBinStream?.DataStream == null)
                return OperationResult.Fail("Entity Data Stream is NULL");

            OperationResult result = OperationResult.Fail("Not yet started");

            await
                new Func<Task>(async () =>
                {
                    DataBinMeta existing = (await (this as ImAStorageService<Guid, DataBinMeta>).LoadByID(entity.ID)).Payload;
                    if (existing != null)
                    {
                        string[] dataBatchIDsToDelete = existing.Notes?.Where(x => x.ID == "DataBatch").OrderBy(x => x.Value).Select(x => x.Value).ToNoNullsArray();
                        if (dataBatchIDsToDelete?.Any() == true)
                        {
                            SetProgressReporterInterval(new NumberInterval(0, dataBatchIDsToDelete.Length));
                            await ReportProgress("Removing existing data bin", 0);
                            await dataBinPayloadStorageService.DeleteBatch(dataBatchIDsToDelete, dataBinPayloadCollectionName);
                            await ReportProgress("Removing existing data bin", dataBatchIDsToDelete.Length);
                        }
                    }

                    List<string> batchIDs = new List<string>();
                    int batchCount = 0;
                    long sizeInBytes = 0;
                    using (Stream stream = dataBinStream.DataStream)
                    {
                        SetProgressReporterInterval(new NumberInterval(0, stream.Length));
                        byte[] batch = new byte[maxDataSizeInBytes];
                        int bytesRead = 0;
                        int batchIndex = -1;
                        do
                        {
                            batchIndex++;

                            bytesRead = 0;
                            int lastBytesRead = 0;
                            do
                            {
                                lastBytesRead = await stream.ReadAsync(batch, bytesRead, maxDataSizeInBytes - bytesRead);
                                bytesRead += lastBytesRead;
                            } while (lastBytesRead > 0 && bytesRead < maxDataSizeInBytes);

                            if (bytesRead == 0)
                                break;

                            sizeInBytes += bytesRead;
                            string bytesReadAsString = Convert.ToBase64String(batch, 0, bytesRead);
                            DataBinPayload dataBinPayload = new DataBinPayload
                            {
                                ID = PrintDataBinPayloadID(entity.ID, batchIndex),
                                DataBinID = entity.ID,
                                BatchIndex = batchIndex,
                                DataPart = bytesReadAsString,
                            };
                            OperationResult batchSaveResult = await dataBinPayloadStorageService.Save(dataBinPayload.ToFirestoreDocument(), dataBinPayloadCollectionName);
                            if (!batchSaveResult.IsSuccessful)
                            {
                                result = batchSaveResult;
                                return;
                            }
                            batchIDs.Add(dataBinPayload.ID);
                            batchCount++;
                            await ReportProgress("Saving new data bin", maxDataSizeInBytes * batchIndex + bytesRead);
                        }
                        while (true);
                    }

                    if (batchCount > 0)
                    {
                        entity.Notes
                            = entity
                            .Notes
                            .Push(sizeInBytes.ToString().NoteAs("SizeInBytes"), checkDistinct: false)
                            .Push(batchCount.ToString().NoteAs("DataBatchCount"), checkDistinct: false)
                            .Push(batchIDs.Select(id => id.NoteAs("DataBatch")), checkDistinct: false)
                            ;
                    }

                    OperationResult metaSaveResult = await (this as ImAStorageService<Guid, DataBinMeta>).Save(entity.ToMeta());
                    if (!metaSaveResult.IsSuccessful)
                    {
                        result = metaSaveResult;
                        return;
                    }

                    result = OperationResult.Win();
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        string message = $"Error occurred while trying to save DataBin ({entity.ID}) to FirestoreDB";
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
                        string message = $"Error occurred while trying to load DataBin ({id}) from FirestoreDB";
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
                        string message = $"Error occurred while trying to load {ids?.Length ?? 0} DataBins from FirestoreDB";
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
                        string message = $"Error occurred while trying to filter and load DataBins page from FirestoreDB";
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
                        string message = $"Error occurred while trying to filter and stream DataBins from FirestoreDB";
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
                        string message = $"Error occurred while trying to stream DataBins from FirestoreDB";
                        await logger.LogError($"{message} ({ex.Message})", ex);
                        result = OperationResult.Fail(ex, message).WithoutPayload<IDisposableEnumerable<DataBin>>();
                    }
                );

            return result;
        }


        private async Task<ImADataBinStream> OpenDataBinContentStream(DataBinMeta meta)
        {
            ImADataBinStream result = null;

            if (meta == null)
                return result;

            await
                new Func<Task>(async () =>
                {
                    long? sizeInBytes = meta.Notes?.Get("SizeInBytes")?.ParseToLongOrFallbackTo(null);
                    if (sizeInBytes == null)
                    {
                        result = null;
                        await logger.LogError($"Cannot open DataBin content stream for {meta.ID} because the metadata has NO Size specified");
                        return;
                    }

                    int? dataBatchCount = meta.Notes?.Get("DataBatchCount")?.ParseToIntOrFallbackTo(null);
                    if (dataBatchCount == null)
                    {
                        result = null;
                        await logger.LogError($"Cannot open DataBin content stream for {meta.ID} because the metadata has NO Data Batch Count");
                        return;
                    }
                    if (dataBatchCount == 0)
                    {
                        result = null;
                        await logger.LogError($"Cannot open DataBin content stream for {meta.ID} because the metadata has Zero Data Bacthes");
                        return;
                    }

                    string[] dataBatchIDs = meta.Notes?.Where(x => x.ID == "DataBatch").OrderBy(x => x.Value).Select(x => x.Value).ToNoNullsArray();
                    if (dataBatchIDs == null)
                    {
                        result = null;
                        await logger.LogError($"Cannot open DataBin content stream for {meta.ID} because the data batch IDs cannot be found in the metadata notes");
                        return;
                    }
                    if (dataBatchIDs.Length != dataBatchCount)
                    {
                        result = null;
                        await logger.LogError($"Cannot open DataBin content stream for {meta.ID} because the number data batch IDs doesn't match the Data Batch Count");
                        return;
                    }

                    PayloadBatchedReadStream dataStream = new PayloadBatchedReadStream(sizeInBytes.Value, maxDataSizeInBytes, LoadDataBinPayloadBatch, dataBatchIDs);

                    result = dataStream.ToDataBinStream();
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        await logger.LogError($"Error occurred while trying to open DataBin content stream for {meta.ID} from FirestoreDB. Message: {ex.Message}", ex, meta);
                        result = null;
                    }
                );

            return result;
        }
        private static string PrintDataBinPayloadID(Guid dataBinID, int batchIndex)
        {
            return $"{dataBinID}-DataBatch-{batchIndex.ToString().PadLeft(5, '0')}";
        }
        private void SetProgressReporterInterval(NumberInterval interval)
        {
            ProgressReporter reporter = ProgressReporter.Get(WellKnownFirestoreCallContextKey.FirestoreDataBinProgressionScopeID);
            if (reporter == null)
                return;

            reporter.SetSourceInterval(interval);
        }
        private async Task ReportProgress(string action, decimal progressValue)
        {
            ProgressReporter reporter = ProgressReporter.Get(WellKnownFirestoreCallContextKey.FirestoreDataBinProgressionScopeID);
            if (reporter == null)
                return;

            await reporter.RaiseOnProgress(action, progressValue);
        }
        private async Task<DataBinPayload> LoadDataBinPayloadBatch(string batchID)
        {
            HsFirestoreDocument batchDocument
                = (await dataBinPayloadStorageService.Load(batchID, dataBinPayloadCollectionName))
                .ThrowOnFailOrReturn()
                ;

            return
                batchDocument
                .ToData<DataBinPayload>()
                ;
        }



        public class DataBinPayload : IStringIdentity
        {
            public string ID { get; set; }
            public Guid DataBinID { get; set; }
            public int BatchIndex { get; set; }
            public string DataPart { get; set; }
        }

        public class PayloadBatchedReadStream : Stream
        {
            long currentPosition = 0;
            int currentBatchIndex = -1;
            byte[] remainingBytesInCurrentDataPartBatch = Array.Empty<byte>();
            readonly long sizeInBytes = 0;
            readonly int maxBatchSize = 0;
            readonly Func<string, Task<DataBinPayload>> dataBinPayloadBatchLoader;
            readonly string[] dataBatchIDs;
            public PayloadBatchedReadStream(long sizeInBytes, int maxBatchSize, Func<string, Task<DataBinPayload>> dataBinPayloadBatchLoader, string[] dataBatchIDs)
            {
                this.sizeInBytes = sizeInBytes;
                this.maxBatchSize = maxBatchSize;
                this.dataBinPayloadBatchLoader = dataBinPayloadBatchLoader;
                this.dataBatchIDs = dataBatchIDs;
                SetProgressReporterInterval(new NumberInterval(0, sizeInBytes));
            }

            private long RemainingBytesToRead => sizeInBytes - currentPosition;
            private int RemainingBatchesToBeRead => dataBatchIDs.Length - currentBatchIndex - 1;
            private string[] RemainingBatchIDs => dataBatchIDs.Jump(currentBatchIndex + 1);

            public override bool CanRead => true;

            public override bool CanSeek => false;

            public override bool CanWrite => false;

            public override long Length => sizeInBytes;

            public override long Position { get => currentPosition; set { } }

            public override void Flush() { }

            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

            public override void SetLength(long value) { }

            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

            public override int Read(byte[] buffer, int offset, int count)
            {
                if (count <= 0)
                    throw new ArgumentException("Requested read count cannot be <= 0. It makes no sense.", nameof(count));
                if (offset + count > buffer.Length)
                    throw new ArgumentException("Requested read offset + count >= buffer.Length, therefore the read piece will exceed the available buffer.", nameof(buffer));

                if (RemainingBytesToRead <= 0)
                    return 0;

                int totalBytesReadInCurrentEpoch = 0;
                int currentOffsetInCurrentEpoch = offset;
                int remainingCountInCurrentEpoch = count;

                do
                {
                    if (remainingBytesInCurrentDataPartBatch?.Any() == true)
                    {
                        int lengthToCopy = Math.Min(remainingBytesInCurrentDataPartBatch.Length, remainingCountInCurrentEpoch);
                        Array.Copy(
                            sourceArray: remainingBytesInCurrentDataPartBatch,
                            sourceIndex: 0,
                            destinationArray: buffer,
                            destinationIndex: currentOffsetInCurrentEpoch,
                            length: lengthToCopy
                        );
                        totalBytesReadInCurrentEpoch += lengthToCopy;
                        currentOffsetInCurrentEpoch += lengthToCopy;
                        remainingCountInCurrentEpoch -= lengthToCopy;
                        currentPosition += lengthToCopy;
                        remainingBytesInCurrentDataPartBatch = remainingBytesInCurrentDataPartBatch.Jump(lengthToCopy).NullIfEmpty();
                        ReportProgress("Streaming data bin", currentPosition);

                        if (remainingCountInCurrentEpoch == 0)
                            break;

                        if (RemainingBytesToRead == 0)
                            break;
                    }


                    int amountOfBatchesToBeRead = remainingCountInCurrentEpoch / maxBatchSize + 1;
                    if (amountOfBatchesToBeRead > RemainingBatchesToBeRead)
                        amountOfBatchesToBeRead = RemainingBatchesToBeRead;
                    string[] batchIDsToRead = RemainingBatchIDs.Take(amountOfBatchesToBeRead).ToArray();
                    foreach (string batchID in batchIDsToRead)
                    {
                        string bytesAsString = LoadDataBatch(batchID).DataPart;
                        currentBatchIndex++;
                        byte[] partAsBytes = Convert.FromBase64String(bytesAsString);
                        int lengthToCopy = Math.Min(partAsBytes.Length, remainingCountInCurrentEpoch);
                        Array.Copy(
                            sourceArray: partAsBytes,
                            sourceIndex: 0,
                            destinationArray: buffer,
                            destinationIndex: currentOffsetInCurrentEpoch,
                            length: lengthToCopy
                        );
                        totalBytesReadInCurrentEpoch += lengthToCopy;
                        currentOffsetInCurrentEpoch += lengthToCopy;
                        remainingCountInCurrentEpoch -= lengthToCopy;
                        currentPosition += lengthToCopy;
                        remainingBytesInCurrentDataPartBatch = partAsBytes.Jump(lengthToCopy).NullIfEmpty();
                        ReportProgress("Streaming data bin", currentPosition);
                    }

                    if (remainingCountInCurrentEpoch == 0)
                        break;

                    if (RemainingBytesToRead == 0)
                        break;

                } while (true);

                return totalBytesReadInCurrentEpoch;
            }

            private DataBinPayload LoadDataBatch(string batchID)
            {
                return
                    dataBinPayloadBatchLoader
                    .Invoke(batchID)
                    .ConfigureAwait(continueOnCapturedContext: false)
                    .GetAwaiter()
                    .GetResult()
                    ;
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
            }

            private void SetProgressReporterInterval(NumberInterval interval)
            {
                ProgressReporter reporter = ProgressReporter.Get(WellKnownFirestoreCallContextKey.FirestoreDataBinProgressionScopeID);
                if (reporter == null)
                    return;

                reporter.SetSourceInterval(interval);
            }
            private void ReportProgress(string action, decimal progressValue)
            {
                ProgressReporter reporter = ProgressReporter.Get(WellKnownFirestoreCallContextKey.FirestoreDataBinProgressionScopeID);
                if (reporter == null)
                    return;

                reporter.RaiseOnProgress(action, progressValue).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter().GetResult();
            }
        }
    }
}
