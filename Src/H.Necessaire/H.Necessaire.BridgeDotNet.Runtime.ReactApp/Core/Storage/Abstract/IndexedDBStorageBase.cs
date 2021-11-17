using Bridge;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Retyped.dexie;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public abstract class IndexedDBStorageBase : ImAnIndexedDBStorage
    {
        #region Construct
        public string DatabaseName { get; }
        public int DatabaseVersion { get; }

        readonly IndexedDBVersionInfo[] versions;
        protected readonly Dexie database;
        public IndexedDBStorageBase(string databaseName, params IndexedDBVersionInfo[] versions)
        {
            DatabaseName = databaseName;
            DatabaseVersion = versions.Max(x => x.Version);
            this.versions = versions;
            this.database = EnsureDatabase();
        }
        #endregion

        public Task<long> Count(Dexie.Table<object, object> table)
        {
            TaskCompletionSource<long> taskCompletionSource = new TaskCompletionSource<long>();

            table
                .count()
                .then(
                    count =>
                    {
                        taskCompletionSource.SetResult((long)count);
                        return count;
                    },
                    x =>
                    {
                        taskCompletionSource.SetException(new InvalidOperationException($"Error counting {table.name}"));
                        return null;
                    }
                );

            return taskCompletionSource.Task;
        }

        public Task<Dexie.Table<object, object>[]> GetAllTables()
        {
            TaskCompletionSource<Dexie.Table<object, object>[]> taskCompletionSource
                = new TaskCompletionSource<Dexie.Table<object, object>[]>();

            if (!database.isOpen())
            {
                database.open().then(_ =>
                {
                    taskCompletionSource.SetResult(database.tables);
                    return database;
                }, x =>
                {
                    taskCompletionSource.SetException(new InvalidOperationException($"Error opening database {DatabaseName} v{DatabaseVersion}"));
                    return null;
                });
            }
            else
            {
                taskCompletionSource.SetResult(database.tables);
            }

            return taskCompletionSource.Task;
        }

        public Dexie.Table<object, object> GetTable(string tableName)
        {
            return database.table(tableName);
        }

        private Dexie EnsureDatabase()
        {
            Dexie db = new Dexie(DatabaseName);

            foreach (IndexedDBVersionInfo version in versions.OrderBy(x => x.Version))
            {
                Dexie.Version dbVersion = Script.Call<Dexie.Version>("db.version", version.Version);
                dbVersion = dbVersion.stores(version.ConstructDbVersionSchema());
                if (version.VersionMigrationProcessor != null)
                    dbVersion.upgrade(version.VersionMigrationProcessor);
            }

            return db;
        }
    }
}
