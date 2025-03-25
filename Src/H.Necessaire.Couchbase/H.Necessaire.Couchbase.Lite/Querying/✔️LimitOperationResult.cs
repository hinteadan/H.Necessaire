using Couchbase.Lite.Query;
using H.Necessaire.Couchbase.Lite.Abstract;
using System;
using System.Linq.Expressions;

namespace H.Necessaire.Couchbase.Lite.Querying
{
    public class LimitOperationResult<T> : CouchbaseQueryBase<ILimit>
    {
        readonly Expression<Func<T, long>> limit;
        readonly Expression<Func<T, long>> offset = null;
        readonly long limitValue;
        readonly long? offsetValue;
        public LimitOperationResult(Expression<Func<T, long>> limit, Expression<Func<T, long>> offset, ICouchbaseQuery parent) : base(parent)
        {
            this.limit = limit;
            this.offset = offset;
        }
        public LimitOperationResult(long limit, long? offset, ICouchbaseQuery parent) : base(parent)
        {
            limitValue = limit;
            offsetValue = offset;
        }

        protected override ILimit CompileQuery(IQuery querySoFar)
        {
            if (IsValueDefiniton())
            {
                return
                    offsetValue is null
                    ? (querySoFar as ILimitRouter).Limit(CouchbaseLinqExpressionInterpreter.Instance.Limit<T>(limitValue))
                    : (querySoFar as ILimitRouter).Limit(CouchbaseLinqExpressionInterpreter.Instance.Limit<T>(limitValue), CouchbaseLinqExpressionInterpreter.Instance.Offset<T>(offsetValue))
                    ;
            }

            return
                offset is null
                ? (querySoFar as ILimitRouter).Limit(CouchbaseLinqExpressionInterpreter.Instance.Limit(limit))
                : (querySoFar as ILimitRouter).Limit(CouchbaseLinqExpressionInterpreter.Instance.Limit(limit), CouchbaseLinqExpressionInterpreter.Instance.Offset(offset))
                ;
        }

        bool IsValueDefiniton() => limit is null;
    }
}
