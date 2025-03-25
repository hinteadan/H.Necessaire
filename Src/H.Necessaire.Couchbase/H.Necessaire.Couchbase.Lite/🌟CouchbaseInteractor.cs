using Couchbase.Lite.Logging;
using System.IO;

namespace H.Necessaire.Couchbase.Lite
{
    public class CouchbaseInteractor
    {
        readonly string databaseName;
        readonly string databaseFolderPath;
        public CouchbaseInteractor(string databaseName, string databaseFolderPath = null)
        {
            this.databaseName = databaseName;
            this.databaseFolderPath = databaseFolderPath;

            LogLevel logLevel = LogLevel.Warning;
#if DEBUG
            logLevel = LogLevel.Debug;
#endif
            LogSinks.File = new FileLogSink(logLevel, Path.Combine(this.databaseFolderPath.IsEmpty() ? "" : this.databaseFolderPath, "CouchbaseLogs"));
        }

        public CouchbaseOperations NewOperationScope(string collectionName, string scopeName)
            => new CouchbaseOperations(databaseFolderPath.IsEmpty() ? new CouchbaseOperationScope(databaseName, scopeName, collectionName) : new CouchbaseOperationScope(databaseName, databaseFolderPath, scopeName, collectionName));

        public CouchbaseOperations NewOperationScope(string collectionName)
            => new CouchbaseOperations(databaseFolderPath.IsEmpty() ? new CouchbaseOperationScope(databaseName, collectionName) : new CouchbaseOperationScope(databaseName, new DirectoryInfo(databaseFolderPath), collectionName));

        public CouchbaseOperations NewOperationScope()
            => new CouchbaseOperations(databaseFolderPath.IsEmpty() ? new CouchbaseOperationScope(databaseName) : new CouchbaseOperationScope(databaseName, new DirectoryInfo(databaseFolderPath)));
    }
}
