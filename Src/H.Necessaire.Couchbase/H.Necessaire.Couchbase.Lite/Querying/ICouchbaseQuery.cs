using Couchbase.Lite.Query;

namespace H.Necessaire.Couchbase.Lite.Querying
{
    public interface ICouchbaseQuery
    {
        IQuery ToQuery();
        IQuery ToCountQuery();
    }
}
