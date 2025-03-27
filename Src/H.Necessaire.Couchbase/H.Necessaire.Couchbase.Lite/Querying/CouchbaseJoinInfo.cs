using Couchbase.Lite;
using System;
using System.Linq.Expressions;

namespace H.Necessaire.Couchbase.Lite.Querying
{
    public class CouchbaseJoinInfo<TThis, TThat>
    {
        public CouchbaseJoinInfo(Collection collection, string collectionAlias, Expression<Func<TThis, TThat, bool>> joinBy, CouchbaseJoinType joinType = CouchbaseJoinType.LeftOuter)
        {
            JoinType = joinType;
            Collection = collection;
            CollectionAlias = collectionAlias;
            JoinBy = joinBy;
        }

        public CouchbaseJoinInfo(Collection collection, Expression<Func<TThis, TThat, bool>> joinBy, CouchbaseJoinType joinType = CouchbaseJoinType.LeftOuter)
            : this(collection, null, joinBy, joinType) { }

        public CouchbaseJoinType JoinType { get; } = CouchbaseJoinType.LeftOuter;
        public Collection Collection { get; }
        public string CollectionAlias { get; }
        public Expression<Func<TThis, TThat, bool>> JoinBy { get; }

        public static implicit operator CouchbaseJoinInfo<TThis, TThat>((Collection, Expression<Func<TThis, TThat, bool>>) joinBy)
            => new CouchbaseJoinInfo<TThis, TThat>(joinBy.Item1, joinBy.Item2);
    }
}
