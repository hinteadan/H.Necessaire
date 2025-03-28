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

        public static implicit operator CouchbaseJoinInfo<TThis, TThat>((Collection collection, Expression<Func<TThis, TThat, bool>> joinExpression) joinBy)
            => new CouchbaseJoinInfo<TThis, TThat>(joinBy.collection, joinBy.joinExpression);
        public static implicit operator CouchbaseJoinInfo<TThis, TThat>((Collection collection, string collectionAlias, Expression<Func<TThis, TThat, bool>> joinExpression) joinBy)
            => new CouchbaseJoinInfo<TThis, TThat>(joinBy.collection, joinBy.collectionAlias, joinBy.joinExpression);
        public static implicit operator CouchbaseJoinInfo<TThis, TThat>((Collection collection, string collectionAlias, Expression<Func<TThis, TThat, bool>> joinExpression, CouchbaseJoinType joinType) joinBy)
            => new CouchbaseJoinInfo<TThis, TThat>(joinBy.collection, joinBy.collectionAlias, joinBy.joinExpression, joinBy.joinType);
        public static implicit operator CouchbaseJoinInfo<TThis, TThat>((Collection collection, Expression<Func<TThis, TThat, bool>> joinExpression, CouchbaseJoinType joinType) joinBy)
            => new CouchbaseJoinInfo<TThis, TThat>(joinBy.collection, joinBy.joinExpression, joinBy.joinType);
    }
}
