using Couchbase.Lite.Query;
using H.Necessaire.Couchbase.Lite.Querying;

namespace H.Necessaire.Couchbase.Lite.Abstract
{
    public abstract class CouchbaseQueryBase<TQuery> : ICouchbaseQuery where TQuery : IQuery
    {
        readonly IDataSource dataSource = null;
        readonly ISelectResult[] selects = null;
        readonly ICouchbaseQuery parent = null;
        protected CouchbaseQueryBase(IDataSource dataSource, ISelectResult[] selects)
        {
            this.dataSource = dataSource;
            this.selects = selects;
        }
        protected CouchbaseQueryBase(ICouchbaseQuery parent)
        {
            this.parent = parent;
        }
        protected abstract TQuery CompileQuery(IQuery querySoFar);

        public IQuery ToQuery() => Compile();
        public IQuery ToCountQuery() => CompileCount();

        protected TQuery Compile() => CompileQuery(parent?.ToQuery() ?? CompileSelect());

        protected TQuery CompileCount() => CompileQuery(parent?.ToCountQuery() ?? CompileCountSelect());

        IFrom CompileSelect() => QueryBuilder.Select(selects).From(dataSource);
        IFrom CompileCountSelect() => QueryBuilder.Select(CouchbaseLinqExpressionInterpreter.Instance.SelectCount()).From(dataSource);
    }
}
