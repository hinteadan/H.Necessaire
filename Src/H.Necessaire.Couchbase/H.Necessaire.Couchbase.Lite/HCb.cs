using Couchbase.Lite.Query;
using H.Necessaire.Couchbase.Lite.Querying;

namespace H.Necessaire.Couchbase.Lite
{
    public static class HCb
    {
        static readonly CouchbaseLinqExpressionInterpreter interpreter = CouchbaseLinqExpressionInterpreter.Instance;

        public static ISelectResult Select<T>(this CouchbaseSelectInfo<T> selectInfo) => interpreter.Select(selectInfo);

        public static IJoin Join<TThis, TThat>(this CouchbaseJoinInfo<TThis, TThat> joinInfo) => interpreter.Join(joinInfo);
    }
}