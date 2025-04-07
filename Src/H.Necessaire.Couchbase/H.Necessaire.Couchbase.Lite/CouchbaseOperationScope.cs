using Couchbase.Lite;
using System;
using System.IO;
using System.Linq;

namespace H.Necessaire.Couchbase.Lite
{
    public class CouchbaseOperationScope : IDisposable
    {
        public const string DefaultBlobScope = "HBlob";
        public const string DefaultBlobCollection = "HDataBin";

        public CouchbaseOperationScope(string databaseName, DatabaseConfiguration databaseConfiguration, string scopeName, string collectionName, params CouchbaseJoinCollectionInfo[] joinCollections)
        {
            Database = new Database(databaseName, databaseConfiguration);
            Collection = BuildCollection(scopeName, collectionName);
            JoinCollections
                = joinCollections
                ?.Where(x => x != null)
                ?.Select(x => new TaggedValue<Collection>
                {
                    Value = BuildCollection(x.Scope, x.Collection),
                    Name = x.Alias,
                    ID = x.Alias,
                })
                .ToArrayNullIfEmpty()
                ;
        }

        public CouchbaseOperationScope(string databaseName, string scopeName, string collectionName, params CouchbaseJoinCollectionInfo[] joinCollections)
            : this(databaseName, databaseConfiguration: null, scopeName, collectionName, joinCollections) { }
        public CouchbaseOperationScope(string databaseName, string collectionName, params CouchbaseJoinCollectionInfo[] joinCollections)
            : this(databaseName, databaseConfiguration: null, scopeName: null, collectionName, joinCollections) { }
        public CouchbaseOperationScope(string databaseName, params CouchbaseJoinCollectionInfo[] joinCollections)
            : this(databaseName, databaseConfiguration: null, scopeName: null, collectionName: null, joinCollections) { }

        public CouchbaseOperationScope(string databaseName, string databseFolderPath, string scopeName, string collectionName, params CouchbaseJoinCollectionInfo[] joinCollections)
            : this(databaseName, new DatabaseConfiguration { Directory = databseFolderPath }, scopeName, collectionName, joinCollections) { }
        public CouchbaseOperationScope(string databaseName, DirectoryInfo databseFolder, string scopeName, string collectionName, params CouchbaseJoinCollectionInfo[] joinCollections)
            : this(databaseName, new DatabaseConfiguration { Directory = databseFolder.FullName }, scopeName, collectionName, joinCollections) { }
        public CouchbaseOperationScope(string databaseName, DirectoryInfo databseFolder, string collectionName, params CouchbaseJoinCollectionInfo[] joinCollections)
            : this(databaseName, new DatabaseConfiguration { Directory = databseFolder.FullName }, scopeName: null, collectionName, joinCollections) { }
        public CouchbaseOperationScope(string databaseName, DirectoryInfo databseFolder, params CouchbaseJoinCollectionInfo[] joinCollections)
            : this(databaseName, new DatabaseConfiguration { Directory = databseFolder.FullName }, scopeName: null, collectionName: null, joinCollections) { }

        public Database Database { get; }
        public Collection Collection { get; }
        public TaggedValue<Collection>[] JoinCollections { get; }
        public Scope Scope => Collection?.Scope;

        public CouchbaseOperationScope New(string collectionName = null, string scopeName = null, params CouchbaseJoinCollectionInfo[] joinCollections)
        {
            string databaseName = Database.Name;
            string folder = GetDatabaseRootFolderPath();

            return
                new CouchbaseOperationScope(
                    databaseName,
                    folder.IsEmpty() ? null : new DatabaseConfiguration { Directory = folder },
                    scopeName,
                    collectionName,
                    joinCollections
                );
        }

        public CouchbaseOperationScope DefaultForBlob()
        {
            string databaseName = Database.Name;
            string folder = GetDatabaseRootFolderPath();
            return DefaultForBlob(databaseName, folder);
        }

        public CouchbaseOperationScope DefaultForBlobInCurrentCollection()
        {
            string databaseName = Database.Name;
            string folder = GetDatabaseRootFolderPath();
            string collectionName = Collection.Name;
            return DefaultForBlobInCollection(databaseName, folder, collectionName);
        }

        public static CouchbaseOperationScope DefaultForBlob(string databaseName, DatabaseConfiguration databaseConfiguration)
            => new CouchbaseOperationScope(databaseName, databaseConfiguration, DefaultBlobScope, DefaultBlobCollection);
        public static CouchbaseOperationScope DefaultForBlob(string databaseName, string databseFolderPath)
            => new CouchbaseOperationScope(databaseName, databseFolderPath, DefaultBlobScope, DefaultBlobCollection);
        public static CouchbaseOperationScope DefaultForBlob(string databaseName)
            => new CouchbaseOperationScope(databaseName, DefaultBlobScope, DefaultBlobCollection);

        public static CouchbaseOperationScope DefaultForBlobInCollection(string databaseName, DatabaseConfiguration databaseConfiguration, string collectionName)
            => new CouchbaseOperationScope(databaseName, databaseConfiguration, DefaultBlobScope, collectionName);
        public static CouchbaseOperationScope DefaultForBlobInCollection(string databaseName, string databseFolderPath, string collectionName)
            => new CouchbaseOperationScope(databaseName, databseFolderPath, DefaultBlobScope, collectionName);
        public static CouchbaseOperationScope DefaultForBlobInCollection(string databaseName, string collectionName)
            => new CouchbaseOperationScope(databaseName, DefaultBlobScope, collectionName);

        public void Dispose()
        {
            new Action(Collection.Dispose).TryOrFailWithGrace();
            new Action(Scope.Dispose).TryOrFailWithGrace();

            if(!JoinCollections.IsEmpty())
            {
                foreach (TaggedValue<Collection> joinCollection in JoinCollections)
                {
                    Collection collection = joinCollection.Value;
                    Scope scope = collection.Scope;
                    new Action(collection.Dispose).TryOrFailWithGrace();
                    new Action(scope.Dispose).TryOrFailWithGrace();
                }
            }

            new Action(Database.Close).TryOrFailWithGrace();
            new Action(Database.Dispose).TryOrFailWithGrace();
        }

        string GetDatabaseRootFolderPath()
        {
            if (Database.Config.Directory.IsEmpty())
                return null;

            return Database.Config.Directory;
        }

        Collection BuildCollection(string scopeName, string collectionName)
        {
            return
                scopeName.IsEmpty() && collectionName.IsEmpty()
                ? Database.GetDefaultCollection()
                : scopeName.IsEmpty()
                ? Database.CreateCollection(collectionName)
                : Database.CreateCollection(collectionName, scopeName)
                ;
        }
    }
}
