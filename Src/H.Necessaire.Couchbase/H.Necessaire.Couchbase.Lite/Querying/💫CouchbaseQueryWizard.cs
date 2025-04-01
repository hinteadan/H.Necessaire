using Couchbase.Lite.Query;
using System;
using System.Linq.Expressions;

namespace H.Necessaire.Couchbase.Lite.Querying
{
    public static class CouchbaseQueryWizard
    {
        public static SelectOperationResult<T> SelectAll<T>(this CouchbaseOperationScope operationScope, string alias = null)
        {
            IDataSource dataSource = alias.IsEmpty() ? DataSource.Collection(operationScope.Collection) : DataSource.Collection(operationScope.Collection).As(alias);
            return
                new SelectOperationResult<T>(dataSource, CouchbaseLinqExpressionInterpreter.Instance.SelectAll(alias));
        }

        public static SelectOperationResult<T> SelectCount<T>(this CouchbaseOperationScope operationScope, string alias = null)
        {
            IDataSource dataSource = alias.IsEmpty() ? DataSource.Collection(operationScope.Collection) : DataSource.Collection(operationScope.Collection).As(alias);
            return
                new SelectOperationResult<T>(dataSource, CouchbaseLinqExpressionInterpreter.Instance.SelectCount(alias));
        }

        public static SelectOperationResult<T> Select<T>(this CouchbaseOperationScope operationScope, params Expression<Func<T, object>>[] selectors)
        {
            return
                new SelectOperationResult<T>(DataSource.Collection(operationScope.Collection), CouchbaseLinqExpressionInterpreter.Instance.Select(selectors));
        }

        public static SelectOperationResult<T> Select<T>(this CouchbaseOperationScope operationScope, params ISelectResult[] selects)
        {
            return
                new SelectOperationResult<T>(DataSource.Collection(operationScope.Collection), selects);
        }

        public static SelectOperationResult<T> Select<T>(this CouchbaseOperationScope operationScope, string alias, params Expression<Func<T, object>>[] selectors)
        {
            return
                new SelectOperationResult<T>(DataSource.Collection(operationScope.Collection).As(alias), CouchbaseLinqExpressionInterpreter.Instance.Select(alias, selectors));
        }

        public static SelectOperationResult<T> Select<T>(this CouchbaseOperationScope operationScope, string alias, params ISelectResult[] selects)
        {
            return
                new SelectOperationResult<T>(DataSource.Collection(operationScope.Collection).As(alias), selects);
        }
    }

}
