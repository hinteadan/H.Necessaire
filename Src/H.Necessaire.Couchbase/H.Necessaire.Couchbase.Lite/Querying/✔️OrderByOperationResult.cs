using Couchbase.Lite.Query;
using H.Necessaire.Couchbase.Lite.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace H.Necessaire.Couchbase.Lite.Querying
{
    public class OrderByOperationResult<T> : CouchbaseQueryBase<IOrderBy>
    {
        readonly Queue<KeyValuePair<Expression<Func<T, object>>, bool>> orderByExpressionsQueue = new Queue<KeyValuePair<Expression<Func<T, object>>, bool>>();
        public OrderByOperationResult(Expression<Func<T, object>> orderBy, bool isDesc, ICouchbaseQuery parent) : base(parent)
        {
            orderByExpressionsQueue.Enqueue(new KeyValuePair<Expression<Func<T, object>>, bool>(orderBy, isDesc));
        }

        protected override IOrderBy CompileQuery(IQuery querySoFar)
        {
            return
                (querySoFar as IOrderByRouter)
                .OrderBy(
                    orderByExpressionsQueue
                    .Select(x => CouchbaseLinqExpressionInterpreter.Instance.OrderBy(x.Key, x.Value))
                    .ToNoNullsArray(nullIfEmpty: false)
                );
        }

        public OrderByOperationResult<T> ThenBy(Expression<Func<T, object>> orderBy)
        {
            orderByExpressionsQueue.Enqueue(new KeyValuePair<Expression<Func<T, object>>, bool>(orderBy, false));
            return this;
        }

        public OrderByOperationResult<T> ThenByDesc(Expression<Func<T, object>> orderBy)
        {
            orderByExpressionsQueue.Enqueue(new KeyValuePair<Expression<Func<T, object>>, bool>(orderBy, true));
            return this;
        }

        public LimitOperationResult<T> Limit(Expression<Func<T, long>> limit, Expression<Func<T, long>> offset = null)
        {
            return new LimitOperationResult<T>(limit, offset, this);
        }
        public LimitOperationResult<T> Limit(long limit) => new LimitOperationResult<T>(limit, null, this);
        public LimitOperationResult<T> Limit(long limit, long offset) => new LimitOperationResult<T>(limit, offset, this);


    }
}
