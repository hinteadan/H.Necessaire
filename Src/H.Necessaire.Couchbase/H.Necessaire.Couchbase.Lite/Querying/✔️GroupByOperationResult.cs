using Couchbase.Lite.Query;
using H.Necessaire.Couchbase.Lite.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace H.Necessaire.Couchbase.Lite.Querying
{
    public class GroupByOperationResult<T> : CouchbaseQueryBase<IGroupBy>
    {
        readonly Queue<Expression<Func<T, object>>> groupByExpressionsQueue = new Queue<Expression<Func<T, object>>>();
        public GroupByOperationResult(Expression<Func<T, object>> groupBy, ICouchbaseQuery parent) : base(parent)
        {
            groupByExpressionsQueue.Enqueue(groupBy);
        }

        protected override IGroupBy CompileQuery(IQuery querySoFar)
        {
            return
                (querySoFar as IGroupByRouter)
                .GroupBy(
                    groupByExpressionsQueue
                    .Select(CouchbaseLinqExpressionInterpreter.Instance.GroupBy)
                    .ToNoNullsArray(nullIfEmpty: false)
                );
        }

        public HavingOperationResult<T> Having(Expression<Func<T, bool>> filter)
        {
            return new HavingOperationResult<T>(filter, this);
        }

        public GroupByOperationResult<T> ThenBy(Expression<Func<T, object>> groupBy)
        {
            groupByExpressionsQueue.Enqueue(groupBy);
            return this;
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
