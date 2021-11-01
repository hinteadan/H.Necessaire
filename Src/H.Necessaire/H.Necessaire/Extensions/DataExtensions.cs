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

        public static bool? ParseToBoolOrFallbackTo(this string rawValue, bool? fallbackValue = null)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return fallbackValue;

            bool parseResult;
            if (bool.TryParse(rawValue, out parseResult))
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

        public static bool IsSameOrSubclassOf(this Type typeToCheck, Type typeToCompareWith)
        {
            return
                typeToCheck == typeToCompareWith
                || typeToCompareWith.IsSubclassOf(typeToCheck);
        }

        public static IDisposableEnumerable<T> AsDisposableEnumerable<T>(this IEnumerable<T> collection)
        {
            return new Operations.Concrete.DataStream<T>(collection);
        }

        public static async Task<string> ReadAsStringAsync(this Stream stream, bool isStreamLeftOpen = false, Encoding encoding = null, bool detectEncodingFromByteOrderMarks = false, int bufferSize = 1024)
        {
            if (stream == null)
                return null;

            using (StreamReader reader = new StreamReader(stream, encoding ?? Encoding.UTF8, detectEncodingFromByteOrderMarks, bufferSize, leaveOpen: isStreamLeftOpen))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public static OperationResult Merge(this IEnumerable<OperationResult> operationResults, string globalReasonIfNecesarry = defaultGlobalReasonForMultipleFailedOperations)
        {
            if (!operationResults?.Any() ?? true)
                return OperationResult.Win();

            if (operationResults.All(x => x.IsSuccessful))
                return OperationResult.Win();

            OperationResult[] failedRules = operationResults.Where(x => !x.IsSuccessful).ToArray();

            if (failedRules.Length == 1)
                return OperationResult.Fail(failedRules.Single().Reason);

            return OperationResult.Fail(
                reason: string.IsNullOrWhiteSpace(globalReasonIfNecesarry) ? defaultGlobalReasonForMultipleFailedOperations : globalReasonIfNecesarry,
                comments: failedRules.SelectMany(x => x.FlattenReasons()).ToArray()
                );
        }

        public static Note NoteAs(this string value, string id)
        {
            return new Note(id, value);
        }
    }
}
