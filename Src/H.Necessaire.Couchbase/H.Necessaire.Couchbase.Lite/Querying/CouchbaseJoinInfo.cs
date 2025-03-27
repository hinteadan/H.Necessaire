namespace H.Necessaire.Couchbase.Lite.Querying
{
    public class CouchbaseJoinInfo
    {
        public CouchbaseJoinType JoinType { get; set; } = CouchbaseJoinType.LeftOuter;
        public string CollectionName { get; set; }
        public string CollectionAlias { get; set; }
    }
}
