namespace H.Necessaire.Couchbase.Lite
{
    public class CouchbaseJoinCollectionInfo
    {
        public static CouchbaseJoinCollectionInfo Default => new CouchbaseJoinCollectionInfo();

        public CouchbaseJoinCollectionInfo(string name = null, string scope = null)
        {
            Collection = name;
            Scope = scope;
        }

        public string Collection { get; }
        public string Scope { get; }
        public string Alias { get; private set; }

        public CouchbaseJoinCollectionInfo As(string alias) => this.And(x => x.Alias = alias);

        public static implicit operator CouchbaseJoinCollectionInfo(string name) => new CouchbaseJoinCollectionInfo(name);
    }
}
