using Couchbase.Lite.Query;
using System;
using System.Linq.Expressions;

namespace H.Necessaire.Couchbase.Lite.Querying
{
    public static class CouchbaseQueryWizard
    {
        public static SelectOperationResult<T> SelectAll<T>(this CouchbaseOperationScope operationScope)
        {
            return
                new SelectOperationResult<T>(DataSource.Collection(operationScope.Collection), CouchbaseLinqExpressionInterpreter.Instance.SelectAll());
        }

        public static SelectOperationResult<T> SelectCount<T>(this CouchbaseOperationScope operationScope)
        {
            return
                new SelectOperationResult<T>(DataSource.Collection(operationScope.Collection), CouchbaseLinqExpressionInterpreter.Instance.SelectCount());
        }

        public static SelectOperationResult<T> Select<T>(this CouchbaseOperationScope operationScope, params Expression<Func<T, object>>[] selectors)
        {
            return
                new SelectOperationResult<T>(DataSource.Collection(operationScope.Collection), CouchbaseLinqExpressionInterpreter.Instance.Select(selectors));
        }
    }

}
