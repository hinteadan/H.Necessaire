using System;
using System.Linq.Expressions;

namespace H.Necessaire.Couchbase.Lite.Querying
{
    public class CouchbaseSelectInfo<T>
    {
        private CouchbaseSelectInfo(Expression<Func<T, object>> selector, string fromAlias, string nameAlias)
        {
            Selector = selector;
            FromAlias = fromAlias;
            NameAlias = nameAlias;
        }

        public Expression<Func<T, object>> Selector { get; }
        public string FromAlias { get; }
        public string NameAlias { get; }

        public static implicit operator CouchbaseSelectInfo<T>((Expression<Func<T, object>> selector, string fromAlias, string nameAlias) select)
            => new CouchbaseSelectInfo<T>(select.selector, select.fromAlias, select.nameAlias);
        public static implicit operator CouchbaseSelectInfo<T>((Expression<Func<T, object>> selector, string fromAlias) select)
            => new CouchbaseSelectInfo<T>(select.selector, select.fromAlias, null);
        public static implicit operator CouchbaseSelectInfo<T>(Expression<Func<T, object>> select)
            => new CouchbaseSelectInfo<T>(select, null, null);
    }
}
