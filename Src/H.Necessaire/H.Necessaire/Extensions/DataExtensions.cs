using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public static class DataExtensions
    {
        const string ellipseSymbol = " ... ";
        const string defaultGlobalReasonForMultipleFailedOperations = "There are multiple failure reasons; see comments for details.";
        static Dictionary<string, string> diacriticsDictionaryBulk = new Dictionary<string, string>
        {
            { "äæǽ", "ae" },
            { "öœ", "oe" },
            { "ü", "ue" },
            //{ "Ä", "Ae" },
            { "Ü", "Ue" },
            { "Ö", "Oe" },
            { "ÀÁÂÃÄÅǺĀĂĄǍΑΆẢẠẦẪẨẬẰẮẴẲẶА", "A" },
            { "àáâãåǻāăąǎªαάảạầấẫẩậằắẵẳặа", "a" },
            { "Б", "B" },
            { "б", "b" },
            { "ÇĆĈĊČ", "C" },
            { "çćĉċč", "c" },
            { "Д", "D" },
            { "д", "d" },
            { "ÐĎĐΔ", "Dj" },
            { "ðďđδ", "dj" },
            { "ÈÉÊËĒĔĖĘĚΕΈẼẺẸỀẾỄỂỆЕЭ", "E" },
            { "èéêëēĕėęěέεẽẻẹềếễểệеэ", "e" },
            { "Ф", "F" },
            { "ф", "f" },
            { "ĜĞĠĢΓГҐ", "G" },
            { "ĝğġģγгґ", "g" },
            { "ĤĦ", "H" },
            { "ĥħ", "h" },
            { "ÌÍÎÏĨĪĬǏĮİΗΉΊΙΪỈỊИЫ", "I" },
            { "ìíîïĩīĭǐįıηήίιϊỉịиыї", "i" },
            { "Ĵ", "J" },
            { "ĵ", "j" },
            { "ĶΚК", "K" },
            { "ķκк", "k" },
            { "ĹĻĽĿŁΛЛ", "L" },
            { "ĺļľŀłλл", "l" },
            { "М", "M" },
            { "м", "m" },
            { "ÑŃŅŇΝН", "N" },
            { "ñńņňŉνн", "n" },
            { "ÒÓÔÕŌŎǑŐƠØǾΟΌΩΏỎỌỒỐỖỔỘỜỚỠỞỢО", "O" },
            { "òóôõōŏǒőơøǿºοόωώỏọồốỗổộờớỡởợо", "o" },
            { "П", "P" },
            { "п", "p" },
            { "ŔŖŘΡР", "R" },
            { "ŕŗřρр", "r" },
            { "ŚŜŞȘŠΣС", "S" },
            { "śŝşșšſσςс", "s" },
            { "ȚŢŤŦτТ", "T" },
            { "țţťŧт", "t" },
            { "ÙÚÛŨŪŬŮŰŲƯǓǕǗǙǛỦỤỪỨỮỬỰУ", "U" },
            { "ùúûũūŭůűųưǔǖǘǚǜυύϋủụừứữửựу", "u" },
            { "ÝŸŶΥΎΫỲỸỶỴЙ", "Y" },
            { "ýÿŷỳỹỷỵй", "y" },
            { "В", "V" },
            { "в", "v" },
            { "Ŵ", "W" },
            { "ŵ", "w" },
            { "ŹŻŽΖЗ", "Z" },
            { "źżžζз", "z" },
            { "ÆǼ", "AE" },
            { "ß", "ss" },
            { "Ĳ", "IJ" },
            { "ĳ", "ij" },
            { "Œ", "OE" },
            { "ƒ", "f" },
            { "ξ", "ks" },
            { "π", "p" },
            { "β", "v" },
            { "μ", "m" },
            { "ψ", "ps" },
            { "Ё", "Yo" },
            { "ё", "yo" },
            { "Є", "Ye" },
            { "є", "ye" },
            { "Ї", "Yi" },
            { "Ж", "Zh" },
            { "ж", "zh" },
            { "Х", "Kh" },
            { "х", "kh" },
            { "Ц", "Ts" },
            { "ц", "ts" },
            { "Ч", "Ch" },
            { "ч", "ch" },
            { "Ш", "Sh" },
            { "ш", "sh" },
            { "Щ", "Shch" },
            { "щ", "shch" },
            { "ЪъЬь", "" },
            { "Ю", "Yu" },
            { "ю", "yu" },
            { "Я", "Ya" },
            { "я", "ya" },
        };
        static Dictionary<char, string> diacriticsDictionary = diacriticsDictionaryBulk.SelectMany(kvp => kvp.Key.Select(d => new KeyValuePair<char, string>(d, kvp.Value))).ToDictionary(x => x.Key, x => x.Value);


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

        public static DateTime ToDateTime(this long ticks, DateTimeKind dateTimeKind = DateTimeKind.Utc)
        {
            return new DateTime(ticks, dateTimeKind);
        }

        public static DateTime AsUtcDate(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Utc);
        }

        public static DateTime RoundToMinute(this DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue || dateTime == DateTime.MaxValue)
                return dateTime;

            var secondsToAdd = dateTime.Second >= 30 ? 60 - dateTime.Second : -dateTime.Second;
            var remainderAfterSeconds = dateTime - new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Kind);
            return
                HSafe.Run(() => dateTime.AddSeconds(secondsToAdd) - remainderAfterSeconds).RefPayload(out var result)
                ? result
                : dateTime
                ;
        }

        public static PartialDateTime ToPartialDateTime(this DateTime dateTime)
        {
            return
                new PartialDateTime
                {
                    Year = dateTime.Year,
                    Month = dateTime.Month,
                    DayOfMonth = dateTime.Day,
                    Hour = dateTime.Hour,
                    Minute = dateTime.Minute,
                    Second = dateTime.Second,
                    Millisecond = dateTime.Millisecond,
                    DateTimeKind = dateTime.Kind,
                    WeekDays = null,
                };
        }

        public static PartialDateTime ToPartialDateTime(this DateTime dateTime, Action<PartialDateTime> modifier)
            => dateTime.ToPartialDateTime().And(modifier);

        public static PartialDateTime ToDateOnlyPartialDateTime(this DateTime dateTime)
            => dateTime.ToPartialDateTime(x => { x.Hour = null; x.Minute = null; x.Second = null; x.Millisecond = null; });

        public static PartialDateTime ToTimeOnlyPartialDateTime(this DateTime dateTime)
            => dateTime.ToPartialDateTime(x => { x.Year = null; x.Month = null; x.DayOfMonth = null; });

        public static DateTime ToTimezone(this DateTime dateTime, TimeZoneInfo to)
        {
            if (to is null)
                return dateTime;

            if (dateTime.Kind == DateTimeKind.Utc && to == TimeZoneInfo.Utc)
                return dateTime;

            DateTime utc = dateTime.EnsureUtc();
            DateTime dest = TimeZoneInfo.ConvertTime(utc, TimeZoneInfo.Utc, to);
            return dest;
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

        public static sbyte? ParseToSbyteOrFallbackTo(this string rawValue, sbyte? fallbackValue = null)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return fallbackValue;

            sbyte parseResult;
            if (sbyte.TryParse(rawValue, out parseResult))
                return parseResult;

            return fallbackValue;
        }

        public static byte? ParseToByteOrFallbackTo(this string rawValue, byte? fallbackValue = null)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return fallbackValue;

            byte parseResult;
            if (byte.TryParse(rawValue, out parseResult))
                return parseResult;

            return fallbackValue;
        }

        public static uint? ParseToUIntOrFallbackTo(this string rawValue, uint? fallbackValue = null)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return fallbackValue;

            uint parseResult;
            if (uint.TryParse(rawValue, out parseResult))
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

        public static T RefTo<T>(this T data, out T refVar)
        {
            refVar = data;
            return data;
        }

        public static Action Act<T>(this T data, Action<T> action)
        {
            return () => action?.Invoke(data);
        }

        public static void RegisterTo(this Action action, IList<Action> registry)
        {
            if (action is null)
                return;

            if (registry is null)
                return;

            registry.Add(action);
        }

        public static void Invoke(this IEnumerable<Action> actions)
        {
            if (actions is null)
                return;

            foreach (Action action in actions)
            {
                if (action is null)
                    continue;

                action.Invoke();
            }
        }

        public static TOut Morph<TIn, TOut>(this TIn @in, Func<TIn, TOut> projector, TOut defaultTo = default)
            => projector is null ? defaultTo : projector(@in);

        public static TOut MorphIf<TIn, TOut>(this TIn @in, bool condition, Func<TIn, TOut> projector, TOut defaultTo = default)
            => !condition ? defaultTo : @in.Morph(projector, defaultTo);

        public static TOut MorphIf<TIn, TOut>(this TIn @in, Func<TIn, bool> condition, Func<TIn, TOut> projector, TOut defaultTo = default)
            => condition is null ? defaultTo : @in.MorphIf(condition(@in), projector, defaultTo);

        public static TOut MorphIfNotNull<TIn, TOut>(this TIn @in, Func<TIn, TOut> projector, TOut defaultTo = default) where TIn : class
            => @in.MorphIf(@in != null, projector, defaultTo);

        public static TOut MorphIfNull<TIn, TOut>(this TIn @in, Func<TIn, TOut> projector, TOut defaultTo = default) where TIn : class
            => @in.MorphIf(@in is null, projector, defaultTo);

        public static TOut MorphIfNotEmpty<TOut>(this string @in, Func<string, TOut> projector, TOut defaultTo = default, bool isWhitespaceConsideredEmpty = true)
            => @in.MorphIf(!@in.IsEmpty(isWhitespaceConsideredEmpty), projector, defaultTo);

        public static TOut MorphIfEmpty<TOut>(this string @in, Func<string, TOut> projector, TOut defaultTo = default, bool isWhitespaceConsideredEmpty = true)
            => @in.MorphIf(@in.IsEmpty(isWhitespaceConsideredEmpty), projector, defaultTo);

        public static T Morph<T>(this T @in, Func<T, T> projector) => @in.Morph(projector, @in);
        public static T MorphIf<T>(this T @in, bool condition, Func<T, T> projector) => @in.MorphIf(condition, projector, @in);
        public static T MorphIf<T>(this T @in, Func<T, bool> condition, Func<T, T> projector) => @in.MorphIf(condition, projector, @in);
        public static T MorphIfNotNull<T>(this T @in, Func<T, T> projector) where T : class => @in.MorphIfNotNull(projector, @in);
        public static T MorphIfNull<T>(this T @in, Func<T, T> projector) where T : class => @in.MorphIfNull(projector, @in);
        public static string IfNotEmpty(this string @in, Func<string, string> projector, bool isWhitespaceConsideredEmpty = true) => @in.MorphIfNotEmpty(projector, @in, isWhitespaceConsideredEmpty);
        public static string IfEmpty(this string @in, Func<string, string> projector, bool isWhitespaceConsideredEmpty = true) => @in.MorphIfEmpty(projector, @in, isWhitespaceConsideredEmpty);

        public static T And<T>(this T data, Action<T> doThis) { doThis?.Invoke(data); return data; }
        public static T AndIf<T>(this T data, bool condition, Action<T> doThis)
        {
            if (!condition)
                return data;

            doThis?.Invoke(data);
            return data;
        }
        public static T AndIf<T>(this T data, Func<T, bool> condition, Action<T> doThis)
        {
            if (condition?.Invoke(data) != true)
                return data;

            doThis?.Invoke(data);
            return data;
        }
        public static T IfNotNull<T>(this T data, Action<T> doThis) where T : class
            => data.AndIf(data != null, doThis);

        public static OperationResult Chk<T>(this T data, bool condition, string failReason, string winReason = null)
            => condition ? OperationResult.Win(winReason.NullIfEmpty()).Display(winReason.NullIfEmpty()) : OperationResult.Fail(failReason.NullIfEmpty()).Display(failReason.NullIfEmpty());
        public static OperationResult Chk<T>(this T data, Func<T, bool> condition, string failReason, string winReason = null)
            => data.Chk(condition?.Invoke(data) ?? false, failReason, winReason);
        public static OperationResult Chk<T>(this T data, bool condition, string failDisplay, string winDisplay, string failReason = null, string winReason = null)
            => condition ? OperationResult.Win(winReason.NullIfEmpty() ?? winDisplay.NullIfEmpty()).Display(winDisplay.NullIfEmpty()) : OperationResult.Fail(failReason.NullIfEmpty() ?? failDisplay.NullIfEmpty()).Display(failDisplay.NullIfEmpty());
        public static OperationResult Chk<T>(this T data, Func<T, bool> condition, string failDisplay, string winDisplay, string failReason = null, string winReason = null)
            => data.Chk(condition?.Invoke(data) ?? false, failDisplay, winDisplay, failReason, winReason);

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

        public static IDisposableEnumerable<T> AsDisposableEnumerable<T>(this IEnumerable<T> collection, params IDisposable[] otherDisposables)
        {
            return new Operations.Concrete.DataStream<T>(collection, otherDisposables);
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
            if (operationResults.IsEmpty())
                return OperationResult.Win();

            if (operationResults.All(x => x.IsSuccessful))
                return OperationResult.Win()
                    .Display(operationResults.SelectMany(x => x.ReasonsToDisplay ?? Array.Empty<string>()).ToNonEmptyArray())
                    .Warn(operationResults.SelectMany(x => x.Warnings ?? Array.Empty<string>()).ToNonEmptyArray())
                    ;

            OperationResult[] failedRules = operationResults.Where(x => !x.IsSuccessful).ToArray();

            if (failedRules.Length == 1)
                return OperationResult
                    .Fail(failedRules[0].Reason, failedRules[0].Comments)
                    .Display(failedRules[0].ReasonsToDisplay)
                    .Warn(failedRules[0].Warnings)
                    ;

            return OperationResult.Fail(
                reason: string.IsNullOrWhiteSpace(globalReasonIfNecesarry) ? defaultGlobalReasonForMultipleFailedOperations : globalReasonIfNecesarry,
                comments: failedRules.SelectMany(x => x.FlattenReasons() ?? Array.Empty<string>()).ToNonEmptyArray()
                )
                .Display(failedRules.SelectMany(x => x.ReasonsToDisplay ?? Array.Empty<string>()).ToNonEmptyArray())
                .Warn(failedRules.SelectMany(x => x.Warnings ?? Array.Empty<string>()).ToNonEmptyArray())
                ;
        }

        public static OperationResult ValidateSortFilters(this ISortFilter sortFilter, params string[] validateSortFilters)
        {
            if (!sortFilter.SortFilters?.Any(x => x != null) ?? true)
                return OperationResult.Win();

            if (sortFilter.SortFilters.Where(x => x != null).Any(x => x.By.NotIn(validateSortFilters)))
                return OperationResult.Fail($"Some of the sort properties are invalid. These are the valid sortable properties: {string.Join(", ", validateSortFilters)}.");

            return OperationResult.Win();
        }

        public static TaggedValue<OperationResult> Tag<T>(this OperationResult<T> value, string name, string id = null)
            => new TaggedOperationResult<T> { Value = value, Name = name, ID = id.IsEmpty() ? Guid.NewGuid().ToString() : id };

        public static TaggedValue<T> Tag<T>(this T value, string name = null, string id = null)
            => new TaggedValue<T> { Value = value, Name = name, ID = id.IsEmpty() ? Guid.NewGuid().ToString() : id };

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

        public static ConfigNode ConfigWithSecretFromEmbeddedResources(this string id, string secretName, params Assembly[] assembliesToScan)
        {
            return
                new ConfigNode
                {
                    ID = id,
                    Value = secretName.ReadSecretFromEmbeddedResources(assembliesToScan),
                };
        }

        public static ConfigNode ConfigWithSecretFromEmbeddedResources(this string id, string secretName, string defaultTo, params Assembly[] assembliesToScan)
        {
            return
                new ConfigNode
                {
                    ID = id,
                    Value = secretName.ReadSecretFromEmbeddedResources(defaultTo, assembliesToScan),
                };
        }

        public static ConfigNode ConfigWithSecretFromEmbeddedResources(this string id, string secretName, Encoding encoding, params Assembly[] assembliesToScan)
        {
            return
                new ConfigNode
                {
                    ID = id,
                    Value = secretName.ReadSecretFromEmbeddedResources(encoding, assembliesToScan),
                };
        }

        public static ConfigNode ConfigWithSecretFromEmbeddedResources(this string id, string secretName, string defaultTo, Encoding encoding, params Assembly[] assembliesToScan)
        {
            return
                new ConfigNode
                {
                    ID = id,
                    Value = secretName.ReadSecretFromEmbeddedResources(defaultTo, encoding, assembliesToScan),
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
        public static OperationResult<T> ToFailResult<T>(this T payload, string reason = null, params string[] comments) => OperationResult.Fail(reason, comments).WithPayload(payload);

        public static async Task<OperationResult<T>> ToWinResult<T>(this Task<T> payloadTask, string reason = null, params string[] comments)
        {
            T payload = await payloadTask;
            return payload.ToWinResult(reason, comments);
        }

        public static async Task<OperationResult<T>> ToFailResult<T>(this Task<T> payloadTask, string reason = null, params string[] comments)
        {
            T payload = await payloadTask;
            return payload.ToFailResult(reason, comments);
        }

        public static DataBinMeta ToMeta(this DataBin dataBin)
        {
            if (dataBin is null)
                return null;

            return
                new DataBinMeta
                {
                    AsOf = dataBin.AsOf,
                    Description = dataBin.Description,
                    Format = dataBin.Format,
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
                    Format = dataBinMeta.Format,
                    ID = dataBinMeta.ID,
                    Name = dataBinMeta.Name,
                    Notes = dataBinMeta.Notes,
                };
        }

        public static ImADataBinStream ToDataBinStream(this Stream stream, params IDisposable[] otherDisposables)
        {
            return
                new DataBinStream(stream, otherDisposables);
        }

        public static Stream WithOtherDisposables(this Stream stream, params IDisposable[] otherDisposables)
        {
            return
                new DataBinStream(stream, otherDisposables);
        }

        public static Stream AsStream(this ImADataBinStream stream)
        {
            return stream as Stream;
        }

        public static async Task<Stream> AsStream(this Task<ImADataBinStream> streamTask)
        {
            return (await streamTask) as Stream;
        }

        public static bool Is(this string stringValue, string comparedTo, bool isCaseSensitive = false)
        {
            return string.Equals(stringValue, comparedTo, isCaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsEmpty(this string stringValue, bool isWhitespaceConsideredEmpty = true)
        {
            return isWhitespaceConsideredEmpty ? string.IsNullOrWhiteSpace(stringValue) : string.IsNullOrEmpty(stringValue);
        }

        public static string IfEmpty(this string stringValue, string returnThis, bool isWhitespaceConsideredEmpty = true) => stringValue.IsEmpty(isWhitespaceConsideredEmpty) ? returnThis : stringValue;

        public static bool IsEmpty(this GeoAddressArea geoAddressArea)
        {
            return
                geoAddressArea.Code.IsEmpty()
                && geoAddressArea.Name.IsEmpty()
                ;
        }

        public static GeoAddressArea? NullIfEmpty(this GeoAddressArea geoAddressArea)
        {
            return geoAddressArea.IsEmpty() ? null : geoAddressArea;
        }

        public static bool IsEmpty(this GeoAddressArea? geoAddressArea)
        {
            return
                geoAddressArea is null
                || geoAddressArea.Value.IsEmpty();
        }

        public static GeoAddressArea? NullIfEmpty(this GeoAddressArea? geoAddressArea)
        {
            return geoAddressArea.IsEmpty() ? null : geoAddressArea;
        }

        public static bool IsEmpty(this GeoAddress geoAddress, bool excludeNotes = false)
        {
            if (geoAddress is null)
                return true;

            bool result =
                geoAddress.Continent.IsEmpty()
                && geoAddress.Country.IsEmpty()
                && geoAddress.State.IsEmpty()
                && geoAddress.County.IsEmpty()
                && geoAddress.City.IsEmpty()
                && geoAddress.CityArea.IsEmpty()
                && geoAddress.ZipCode.IsEmpty()
                && geoAddress.StreetAddress.IsEmpty()
                ;

            if (!excludeNotes)
                result = result && (geoAddress.Notes is null || geoAddress.Notes.All(n => n.IsEmpty()));

            return result;
        }

        public static GeoAddress NullIfEmpty(this GeoAddress geoAddress, bool excludeNotes = false)
        {
            return geoAddress.IsEmpty(excludeNotes) ? null : geoAddress;
        }

        public static GeoAddress Curate(this GeoAddress geoAddress, bool nullIfEmpty = true, bool excludeNotes = false)
        {
            if (geoAddress.IsEmpty(excludeNotes) && nullIfEmpty)
            {
                return null;
            }

            geoAddress.Continent = geoAddress.Continent.NullIfEmpty();
            geoAddress.Country = geoAddress.Country.NullIfEmpty();
            geoAddress.State = geoAddress.State.NullIfEmpty();
            geoAddress.County = geoAddress.County.NullIfEmpty();
            geoAddress.City = geoAddress.City.NullIfEmpty();
            geoAddress.CityArea = geoAddress.CityArea.NullIfEmpty();
            geoAddress.ZipCode = geoAddress.ZipCode.NullIfEmpty();
            geoAddress.StreetAddress = geoAddress.StreetAddress.NullIfEmpty();
            geoAddress.Notes = geoAddress.Notes?.Where(n => !n.IsEmpty()).ToArrayNullIfEmpty();

            return geoAddress;
        }

        public static string DiacriticLess(this string value)
        {
            if (value.IsEmpty())
                return value;

            StringBuilder resultBuilder = new StringBuilder(value.Length);

            foreach (char c in value)
            {
                if (diacriticsDictionary.ContainsKey(c) == false)
                {
                    resultBuilder.Append(c);
                    continue;
                }

                resultBuilder.Append(diacriticsDictionary[c]);
            }

            return resultBuilder.ToString();
        }

        public static string EllipsizeIfNecessary(this string value, int maxLength = 30)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            if (value.Length <= maxLength)
                return value;

            if (maxLength <= ellipseSymbol.Length)
                return ellipseSymbol;

            return $"{value.Substring(0, maxLength - ellipseSymbol.Length)}{ellipseSymbol}";
        }

        public static string ToDisplayLabel(this string value)
        {
            if (value.IsEmpty())
                return "";

            if (char.IsLower(value[0]))
                value = $"{char.ToUpper(value[0])}{value.Substring(1)}";

            return Regex.Replace(value, "(\\B([A-Z]|(\\d+)))", " $1");
        }

        public static TimeSpan NoLessThan(this TimeSpan timeSpan, TimeSpan minimum)
        {
            return timeSpan < minimum ? minimum : timeSpan;
        }
        public static TimeSpan NoLessThanZero(this TimeSpan timeSpan) => timeSpan.NoLessThan(TimeSpan.Zero);
        public static TimeSpan? NoLessThan(this TimeSpan? timeSpan, TimeSpan? minimum)
        {
            if (minimum is null)
                return timeSpan;

            if (timeSpan is null)
                return minimum;

            return timeSpan.Value.NoLessThan(minimum.Value);
        }
        public static TimeSpan? NoLessThanZero(this TimeSpan? timeSpan) => timeSpan.NoLessThan(TimeSpan.Zero);

        public static bool In(this double value, NumberInterval numberInterval)
        {
            return numberInterval.Contains(value);
        }

        public static TEphemeralType CloneValidityFrom<TEphemeralType>(this TEphemeralType ephemeralType, IEphemeralType ephemeralInfo) where TEphemeralType : IEphemeralType
        {
            if (ephemeralType == null)
                return ephemeralType;

            if (ephemeralInfo == null)
                return ephemeralType;

            ephemeralType.ActiveAsOf(ephemeralInfo.ValidFrom);
            if (ephemeralInfo.ExpiresAt == null)
                ephemeralType.DoNotExpire();
            else
                ephemeralType.ExpireAt(ephemeralInfo.ExpiresAt.Value);

            return ephemeralType;
        }

        public static TEphemeralType CloneEntireEphmeralTypeInfoFrom<TEphemeralType>(this TEphemeralType ephemeralType, IEphemeralType ephemeralInfo) where TEphemeralType : EphemeralTypeBase
        {
            if (ephemeralType == null)
                return ephemeralType;

            if (ephemeralInfo == null)
                return ephemeralType;

            ephemeralType.CreatedAt = ephemeralInfo.CreatedAt;
            ephemeralType.AsOf = ephemeralInfo.AsOf;
            ephemeralType.ValidFrom = ephemeralInfo.ValidFrom;
            ephemeralType.ExpiresAt = ephemeralInfo.ExpiresAt;

            return ephemeralType;
        }

        public static IEnumerable<DateTime> BuildDailyTimestamps(this DateTime from, DateTime to)
        {
            from = from.Date;
            to = to.Date;

            if (from > to)
                yield break;

            while (from <= to)
            {
                yield return from;
                from = from.AddDays(1);
            }
        }

        public static IEnumerable<DateTime> BuildDailyTimestamps(this PeriodOfTime periodOfTime)
        {
            if (periodOfTime.IsInfinite)
                throw new ArgumentException("The specified period must be well defined, it cannot be infinite", nameof(periodOfTime));

            return periodOfTime.From.Value.BuildDailyTimestamps(periodOfTime.To.Value);
        }

        public static string[] ToLines(this string multiLineValue)
        {
            if (multiLineValue.IsEmpty())
                return null;

            return
                multiLineValue
                .Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .ToNonEmptyArray()
                ;
        }

        public static ConfigNode[] ToConfig(this IEnumerable<string> values)
        {
            if (values.IsEmpty())
                return Array.Empty<ConfigNode>();

            return
                values
                .Where(v => !v.IsEmpty())
                .Select((val, i) => i.ToString().ConfigWith(val))
                .ToArray()
                ;
        }
    }
}
