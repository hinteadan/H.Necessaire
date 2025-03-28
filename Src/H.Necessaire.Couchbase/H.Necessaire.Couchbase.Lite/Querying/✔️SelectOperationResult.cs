using Couchbase.Lite.Query;
using H.Necessaire.Couchbase.Lite.Abstract;
using System;
using System.Linq.Expressions;

namespace H.Necessaire.Couchbase.Lite.Querying
{
    public class SelectOperationResult<T> : CouchbaseQueryBase<IFrom>
    {
        readonly IDataSource dataSource;
        readonly ISelectResult[] selects;
        public SelectOperationResult(IDataSource dataSource, params ISelectResult[] selects)
            : base(dataSource, selects)
        {
            this.dataSource = dataSource;
            this.selects = selects;
        }

        protected override IFrom CompileQuery(IQuery querySoFar) => querySoFar as IFrom;

        public JoinOperationResult<T> Join()
        {
            return new JoinOperationResult<T>(this);
        }

        public WhereOperationResult<T> Where(Expression<Func<T, bool>> filter)
        {
            return new WhereOperationResult<T>(filter, this);
        }

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
