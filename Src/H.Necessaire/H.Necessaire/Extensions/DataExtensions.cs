using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public static class DataExtensions
    {
        const string defaultGlobalReasonForMultipleFailedOperations = "There are multiple failure reasons; see comments for details.";

        public static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static long ToUnixTimestamp(this DateTime date)
        {
            TimeSpan t = (date.ToUniversalTime() - UnixEpoch);
            return (long)(t.TotalMilliseconds + 0.5);
        }

        public static DateTime UnixTimeStampToDateTime(this long unixTimeStamp)
        {
            return UnixEpoch.AddSeconds(unixTimeStamp).ToLocalTime();
        }

        public static DateTime UnixTimeStampToDateTime(this double unixTimeStampInMilliseconds)
        {
            return UnixEpoch.AddMilliseconds(unixTimeStampInMilliseconds).ToLocalTime();
        }

        public static DateTime EnsureUtc(this DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
                return dateTime;

            if (dateTime.Kind == DateTimeKind.Unspecified)
                return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

            return dateTime.ToUniversalTime();
        }

        public static float TrimToPercent(this float value)
        {
            return
                value < 0 ? 0
                : value > 100 ? 100
                : value;
        }
        public static float TrimToPercent(this int value) => TrimToPercent((float)value);
        public static float TrimToPercent(this decimal value) => TrimToPercent((float)value);
        public static float TrimToPercent(this double value) => TrimToPercent((float)value);

        public static Guid? ParseToGuidOrFallbackTo(this string rawValue, Guid? fallbackValue = null)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return fallbackValue;

            Guid parseResult;
            if (Guid.TryParse(rawValue, out parseResult))
                return parseResult;

            return fallbackValue;
        }

        public static int? ParseToIntOrFallbackTo(this string rawValue, int? fallbackValue = null)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return fallbackValue;

            int parseResult;
            if (int.TryParse(rawValue, out parseResult))
                return parseResult;

            return fallbackValue;
        }

        public static long? ParseToLongOrFallbackTo(this string rawValue, long? fallbackValue = null)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return fallbackValue;

            long parseResult;
            if (long.TryParse(rawValue, out parseResult))
                return parseResult;

            return fallbackValue;
        }

        public static bool? ParseToBoolOrFallbackTo(this string rawValue, bool? fallbackValue = null)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return fallbackValue;

            bool parseResult;
            if (bool.TryParse(rawValue, out parseResult))
                return parseResult;

            return fallbackValue;
        }

        public static double? ParseToDoubleOrFallbackTo(this string rawValue, double? fallbackValue = null)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return fallbackValue;

            double parseResult;
            if (double.TryParse(rawValue, out parseResult))
                return parseResult;

            return fallbackValue;
        }

        public static decimal? ParseToDecimalOrFallbackTo(this string rawValue, decimal? fallbackValue = null)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return fallbackValue;

            decimal parseResult;
            if (decimal.TryParse(rawValue, out parseResult))
                return parseResult;

            return fallbackValue;
        }

        public static bool IsBetweenInclusive(this DateTime dateTime, DateTime? from, DateTime? to)
        {
            return
                (dateTime >= (from ?? DateTime.MinValue))
                &&
                (dateTime <= (to ?? DateTime.MaxValue));
        }

        public static T And<T>(this T data, Action<T> doThis) { doThis(data); return data; }

        public static Tuple<T1, T2> TupleWith<T1, T2>(this T1 item1, T2 item2) => Tuple.Create<T1, T2>(item1, item2);

        public static async Task<T> AndAsync<T>(this T data, Func<T, Task> doThis) { await doThis(data); return data; }

        public static async Task<T> AndAsync<T>(this Task<T> asyncData, Func<T, Task> doThis)
        {
            T data = await asyncData;
            await doThis(data);
            return data;
        }

        public static async Task<T> AndAsync<T>(this Task<T> asyncData, Action<T> doThis)
        {
            T data = await asyncData;
            doThis(data);
            return data;
        }

        public static IDisposableEnumerable<T> AsDisposableEnumerable<T>(this IEnumerable<T> collection)
        {
            return new Operations.Concrete.DataStream<T>(collection);
        }

        public static async Task<string> ReadAsStringAsync(this Stream stream, bool isStreamLeftOpen = true, Encoding encoding = null, bool detectEncodingFromByteOrderMarks = false, int bufferSize = 1024)
        {
            if (stream == null)
                return null;

            using (StreamReader reader = new StreamReader(stream, encoding ?? Encoding.UTF8, detectEncodingFromByteOrderMarks, bufferSize, leaveOpen: isStreamLeftOpen))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public static Task<Stream> WriteToStreamAsync(this string value, Stream stream, bool isStreamLeftOpen = true, Encoding encoding = null, int bufferSize = 1024)
        {
            if (stream == null)
                return null;

            using (StreamWriter writer = new StreamWriter(stream, encoding ?? Encoding.UTF8, bufferSize, leaveOpen: isStreamLeftOpen))
            {
                writer.Write(value);
            }

            return stream.AsTask();
        }

        public static Stream ToStream(this string value, Encoding encoding = null) => value == null ? null : new MemoryStream((encoding ?? Encoding.UTF8).GetBytes(value));

        public static OperationResult Merge(this IEnumerable<OperationResult> operationResults, string globalReasonIfNecesarry = defaultGlobalReasonForMultipleFailedOperations)
        {
            if (!operationResults?.Any() ?? true)
                return OperationResult.Win();

            if (operationResults.All(x => x.IsSuccessful))
                return OperationResult.Win();

            OperationResult[] failedRules = operationResults.Where(x => !x.IsSuccessful).ToArray();

            if (failedRules.Length == 1)
                return OperationResult.Fail(failedRules.Single().Reason, failedRules.Single().Comments);

            return OperationResult.Fail(
                reason: string.IsNullOrWhiteSpace(globalReasonIfNecesarry) ? defaultGlobalReasonForMultipleFailedOperations : globalReasonIfNecesarry,
                comments: failedRules.SelectMany(x => x.FlattenReasons()).ToArray()
                );
        }

        public static OperationResult ValidateSortFilters(this ISortFilter sortFilter, params string[] validateSortFilters)
        {
            if (!sortFilter.SortFilters?.Any(x => x != null) ?? true)
                return OperationResult.Win();

            if (sortFilter.SortFilters.Where(x => x != null).Any(x => x.By.NotIn(validateSortFilters)))
                return OperationResult.Fail($"Some of the sort properties are invalid. These are the valid sortable properties: {string.Join(", ", validateSortFilters)}.");

            return OperationResult.Win();
        }

        public static Note NoteAs(this string value, string id)
        {
            return new Note(id, value);
        }

        public static Note[] Add(this Note[] notes, params Note[] additionalNotes)
        {
            return
                (notes ?? new Note[0]).Concat(additionalNotes ?? new Note[0]).Where(x => !x.IsEmpty()).ToArray();
        }

        public static Note[] Add(this Note note, params Note[] additionalNotes) => note.AsArray().Add(additionalNotes);

        public static Note[] AddOrReplace(this Note[] notes, params Note[] additionalNotes) => notes.Add(additionalNotes).GroupBy(x => x.ID).Select(x => x.Last()).ToArray();
        public static Note[] AddOrReplace(this Note note, params Note[] additionalNotes) => note.AsArray().AddOrReplace(additionalNotes);
        public static Note[] ToNotes(this string[] values, string idPrefix = null, int offsetIndex = 0)
        {
            if (values == null) return null;
            if (!values.Any()) return new Note[0];

            return values.Where(x => !string.IsNullOrWhiteSpace(x)).Select((x, i) => x.NoteAs($"{idPrefix}{offsetIndex + i}")).ToArray();
        }

        public static ConfigNode ConfigWith(this string id, string value)
        {
            return
                new ConfigNode
                {
                    ID = id,
                    Value = value,
                };
        }

        public static ConfigNode ConfigWith(this string id, params ConfigNode[] children)
        {
            return
                new ConfigNode
                {
                    ID = id,
                    Value = children,
                };
        }

        public static SyncResponse RespondWith(this SyncRequest syncRequest, SyncStatus syncStatus)
        {
            return
                new SyncResponse
                {
                    ID = syncRequest.ID,
                    PayloadIdentifier = syncRequest.PayloadIdentifier,
                    PayloadType = syncRequest.PayloadType,
                    SyncStatus = syncStatus,
                };
        }

        public static SyncResponse ToFailResponse(this SyncRequest syncRequest) => syncRequest.RespondWith(
            syncRequest.SyncStatus.In(SyncStatus.NotSynced, SyncStatus.Synced) ? SyncStatus.NotSynced
            : syncRequest.SyncStatus.In(SyncStatus.DeletedAndNotSynced, SyncStatus.DeletedAndSynced) ? SyncStatus.DeletedAndNotSynced
            : SyncStatus.Inexistent
            );
        public static SyncResponse ToWinResponse(this SyncRequest syncRequest) => syncRequest.RespondWith(
            syncRequest.SyncStatus.In(SyncStatus.NotSynced, SyncStatus.Synced) ? SyncStatus.Synced
            : syncRequest.SyncStatus.In(SyncStatus.DeletedAndNotSynced, SyncStatus.DeletedAndSynced) ? SyncStatus.DeletedAndSynced
            : SyncStatus.Inexistent
            );

        public static AuditMetadataEntry ToMeta(this ImAnAuditEntry auditEntry) => new AuditMetadataEntry(auditEntry);

        public static AuditMetadataEntry ToAuditMeta<T>(this T data, string dataID, AuditActionType auditActionType = AuditActionType.Modify, IDentity doneBy = null)
        {
            return
                new AuditMetadataEntry
                {
                    AuditedObjectType = typeof(T).TypeName(),
                    AuditedObjectID = dataID ?? Guid.Empty.ToString(),
                    DoneBy = doneBy,
                    ActionType = auditActionType,
                };
        }

        public static AuditMetadataEntry ToAuditMeta<T, TId>(this T data, AuditActionType auditActionType = AuditActionType.Modify, IDentity doneBy = null) where T : IDentityType<TId>
            => data.ToAuditMeta(data.ID?.ToString() ?? Guid.Empty.ToString(), auditActionType, doneBy);

        public static OperationResult<T> ToWinResult<T>(this T payload, string reason = null, params string[] comments) => OperationResult.Win(reason, comments).WithPayload(payload);

        public static DataBinMeta ToMeta(this DataBin dataBin)
        {
            if (dataBin is null)
                return null;

            return
                new DataBinMeta
                {
                    AsOf = dataBin.AsOf,
                    Description = dataBin.Description,
                    Format = dataBin.Format.Clone(),
                    ID = dataBin.ID,
                    Name = dataBin.Name,
                    Notes = dataBin.Notes,
                };
        }

        public static DataBin ToBin(this DataBinMeta dataBinMeta, Func<DataBinMeta, Task<ImADataBinStream>> streamFactory)
        {
            if (dataBinMeta is null)
                return null;

            return
                new DataBin(streamFactory)
                {
                    AsOf = dataBinMeta.AsOf,
                    Description = dataBinMeta.Description,
                    Format = dataBinMeta.Format.Clone(),
                    ID = dataBinMeta.ID,
                    Name = dataBinMeta.Name,
                    Notes = dataBinMeta.Notes,
                };
        }

        public static ImADataBinStream ToDataBinStream(this Stream stream, params IDisposable[] otherDisposables)
        {
            return
                new DefaultDataBinStream(stream, otherDisposables);
        }
    }
}
