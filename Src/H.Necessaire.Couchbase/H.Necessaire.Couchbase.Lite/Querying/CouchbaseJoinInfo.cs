using System;
using System.Linq.Expressions;

namespace H.Necessaire.Couchbase.Lite.Querying
{
    public class CouchbaseJoinInfo<TThis, TThat>
    {
        public CouchbaseJoinInfo(string collectionName, string collectionAlias, Expression<Func<TThis, TThat, bool>> joinBy, CouchbaseJoinType joinType = CouchbaseJoinType.LeftOuter)
        {
            JoinType = joinType;
            CollectionName = collectionName;
            CollectionAlias = collectionAlias;
            JoinBy = joinBy;
        }

        public CouchbaseJoinInfo(string collectionName, Expression<Func<TThis, TThat, bool>> joinBy, CouchbaseJoinType joinType = CouchbaseJoinType.LeftOuter)
            : this(collectionName, collectionName, joinBy, joinType) { }

        public CouchbaseJoinInfo(Expression<Func<TThis, TThat, bool>> joinBy, CouchbaseJoinType joinType = CouchbaseJoinType.LeftOuter)
            : this(typeof(TThat).Name, joinBy, joinType) { }

        public CouchbaseJoinType JoinType { get; } = CouchbaseJoinType.LeftOuter;
        public string CollectionName { get; }
        public string CollectionAlias { get; }
        public Expression<Func<TThis, TThat, bool>> JoinBy { get; }

        public static implicit operator CouchbaseJoinInfo<TThis, TThat>(Expression<Func<TThis, TThat, bool>> joinBy)
            => new CouchbaseJoinInfo<TThis, TThat>(joinBy);
    }
}
