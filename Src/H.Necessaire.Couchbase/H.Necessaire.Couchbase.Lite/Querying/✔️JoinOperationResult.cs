using Couchbase.Lite;
using Couchbase.Lite.Query;
using H.Necessaire.Couchbase.Lite.Abstract;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace H.Necessaire.Couchbase.Lite.Querying
{
    public class JoinOperationResult<T> : CouchbaseQueryBase<IJoins>
    {
        readonly Queue<IJoin> joinersQueue = new Queue<IJoin>();
        public JoinOperationResult(ICouchbaseQuery parent) : base(parent) { }

        protected override IJoins CompileQuery(IQuery querySoFar)
        {
            return
                (querySoFar as IJoinRouter)
                .Join(
                    joinersQueue
                    .ToNoNullsArray(nullIfEmpty: false)
                );
        }


        public JoinOperationResult<T> With<TThis, TThat>(Collection collection, Expression<Func<TThis, TThat, bool>> joinExpression)
        {
            joinersQueue.Enqueue(
                CouchbaseLinqExpressionInterpreter.Instance.Join<TThis, TThat>((collection, joinExpression))
            );
            return this;
        }
        public JoinOperationResult<T> With<TThis, TThat>(Collection collection, string collectionAlias, Expression<Func<TThis, TThat, bool>> joinExpression)
        {
            joinersQueue.Enqueue(
                CouchbaseLinqExpressionInterpreter.Instance.Join<TThis, TThat>((collection, collectionAlias, joinExpression))
            );
            return this;
        }

        public JoinOperationResult<T> With<TThis, TThat>(Collection collection, string collectionAlias, Expression<Func<TThis, TThat, bool>> joinExpression, CouchbaseJoinType joinType)
        {
            joinersQueue.Enqueue(
                CouchbaseLinqExpressionInterpreter.Instance.Join<TThis, TThat>((collection, collectionAlias, joinExpression, joinType))
            );
            return this;
        }

        public JoinOperationResult<T> With<TThis, TThat>(Collection collection, Expression<Func<TThis, TThat, bool>> joinExpression, CouchbaseJoinType joinType)
        {
            joinersQueue.Enqueue(
                CouchbaseLinqExpressionInterpreter.Instance.Join<TThis, TThat>((collection, joinExpression, joinType))
            );
            return this;
        }

        public WhereOperationResult<T> Where(Expression<Func<T, bool>> filter)
        {
            return new WhereOperationResult<T>(filter, this);
        }

        public OrderByOperationResult<T> OrderBy(Expression<Func<T, object>> orderBy, bool isDesc = false)
        {
            return new OrderByOperationResult<T>(orderBy, isDesc, this);
        }

        public LimitOperationResult<T> Limit(Expression<Func<T, long>> limit, Expression<Func<T, long>> offset = null)
        {
            return new LimitOperationResult<T>(limit, offset, this);
        }
        public LimitOperationResult<T> Limit(long limit) => new LimitOperationResult<T>(limit, null, this);
        public LimitOperationResult<T> Limit(long limit, long offset) => new LimitOperationResult<T>(limit, offset, this);
    }
}
