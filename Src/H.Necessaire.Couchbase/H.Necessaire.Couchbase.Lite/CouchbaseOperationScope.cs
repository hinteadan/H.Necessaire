using Couchbase.Lite;
using System;
using System.IO;

namespace H.Necessaire.Couchbase.Lite
{
    public class CouchbaseOperationScope : IDisposable
    {
        public const string DefaultBlobScope = "HBlob";
        public const string DefaultBlobCollection = "HDataBin";

        public CouchbaseOperationScope(string databaseName, DatabaseConfiguration databaseConfiguration, string scopeName, string collectionName)
        {
            Database = new Database(databaseName, databaseConfiguration);
            Collection
                = scopeName.IsEmpty() && collectionName.IsEmpty()
                ? Database.GetDefaultCollection()
                : scopeName.IsEmpty()
                ? Database.CreateCollection(collectionName)
                : Database.CreateCollection(collectionName, scopeName)
                ;
        }
        public CouchbaseOperationScope(string databaseName, string scopeName, string collectionName)
            : this(databaseName, databaseConfiguration: null, scopeName, collectionName) { }
        public CouchbaseOperationScope(string databaseName, string collectionName)
            : this(databaseName, databaseConfiguration: null, scopeName: null, collectionName) { }
        public CouchbaseOperationScope(string databaseName)
            : this(databaseName, databaseConfiguration: null, scopeName: null, collectionName: null) { }

        public CouchbaseOperationScope(string databaseName, string databseFolderPath, string scopeName, string collectionName)
            : this(databaseName, new DatabaseConfiguration { Directory = databseFolderPath }, scopeName, collectionName) { }
        public CouchbaseOperationScope(string databaseName, DirectoryInfo databseFolder, string scopeName, string collectionName)
            : this(databaseName, new DatabaseConfiguration { Directory = databseFolder.FullName }, scopeName, collectionName) { }
        public CouchbaseOperationScope(string databaseName, DirectoryInfo databseFolder, string collectionName)
            : this(databaseName, new DatabaseConfiguration { Directory = databseFolder.FullName }, scopeName: null, collectionName) { }
        public CouchbaseOperationScope(string databaseName, DirectoryInfo databseFolder)
            : this(databaseName, new DatabaseConfiguration { Directory = databseFolder.FullName }, scopeName: null, collectionName: null) { }

        public Database Database { get; }
        public Collection Collection { get; }
        public Scope Scope => Collection?.Scope;

        public CouchbaseOperationScope New(string collectionName = null, string scopeName = null)
        {
            string databaseName = Database.Name;
            string folder = GetDatabaseRootFolderPath();

            return
                new CouchbaseOperationScope(
                    databaseName,
                    folder.IsEmpty() ? null : new DatabaseConfiguration { Directory = folder },
                    scopeName,
                    collectionName
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
            new Action(Database.Close).TryOrFailWithGrace();
            new Action(Database.Dispose).TryOrFailWithGrace();
        }

        string GetDatabaseRootFolderPath()
        {
            if (Database.Config.Directory.IsEmpty())
                return null;

            return Database.Config.Directory;
        }
    }
}
