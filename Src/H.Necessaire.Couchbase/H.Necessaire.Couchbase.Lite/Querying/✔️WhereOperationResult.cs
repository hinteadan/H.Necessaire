using Couchbase.Lite.Query;
using H.Necessaire.Couchbase.Lite.Abstract;
using System;
using System.Linq.Expressions;

namespace H.Necessaire.Couchbase.Lite.Querying
{
    public class WhereOperationResult<T> : CouchbaseQueryBase<IWhere>
    {
        readonly Expression<Func<T, bool>> filter;
        public WhereOperationResult(Expression<Func<T, bool>> filter, ICouchbaseQuery parent) : base(parent)
        {
            this.filter = filter;
        }
        protected override IWhere CompileQuery(IQuery querySoFar)
            => (querySoFar as IWhereRouter).Where(CouchbaseLinqExpressionInterpreter.Instance.Where(filter));

        public GroupByOperationResult<T> GroupBy(Expression<Func<T, object>> groupBy)
        {
            return new GroupByOperationResult<T>(groupBy, this);
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
