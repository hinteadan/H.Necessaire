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
        public JoinOperationResult(ICouchbaseQuery parent) : base(parent)
        {
            throw new NotImplementedException();
        }

        protected override IJoins CompileQuery(IQuery querySoFar)
        {
            return
                (querySoFar as IJoinRouter)
                .Join(
                    joinersQueue
                    .ToNoNullsArray(nullIfEmpty: false)
                );
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
