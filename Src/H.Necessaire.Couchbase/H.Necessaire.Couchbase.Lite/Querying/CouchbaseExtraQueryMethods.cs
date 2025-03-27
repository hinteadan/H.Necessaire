using System.Linq.Expressions;

namespace H.Necessaire.Couchbase.Lite.Querying
{
    public static class CouchbaseExtraQueryMethods
    {
        public static void Average(this Expression expression) { }
        public static void Avg(this Expression expression) { }
        public static void Ceil(this Expression expression) { }
        public static long Count() => 0;
        public static void Count(this Expression expression) { }
        public static void Degrees(this Expression expression) { }
        public static void RadiansToDegrees(this Expression expression) { }
        public static void RadToDeg(this Expression expression) { }
        public static void RadDeg(this Expression expression) { }
        public static void MillisToString(this Expression expression) { }
        public static void MsToString(this Expression expression) { }
        public static void MillisToUTC(this Expression expression) { }
        public static void MsToUTC(this Expression expression) { }
        public static void Radians(this Expression expression) { }
        public static void DegreesToRadians(this Expression expression) { }
        public static void DegToRad(this Expression expression) { }
        public static void DegRad(this Expression expression) { }
        public static void StringToMillis(this Expression expression) { }
        public static void StringToMs(this Expression expression) { }
        public static void StringToUTC(this Expression expression) { }
        public static void Sum(this Expression expression) { }
        public static void Trunc(this Expression expression) { }
        public static void Like(this Expression left, Expression right) { }
        public static bool Like(this string left, string right) => true;
        public static void Regex(this Expression left, Expression right) { }
        public static bool Regex(this string left, string right) => true;
        public static T FromAlias<T>(this T any, string alias) => any;
    }
}
