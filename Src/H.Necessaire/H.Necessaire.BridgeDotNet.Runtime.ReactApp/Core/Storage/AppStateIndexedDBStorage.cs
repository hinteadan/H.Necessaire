using H.Necessaire.BridgeDotNet.Runtime.ReactApp.Resources;
using System.Threading.Tasks;
using static Retyped.dexie;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp.Core.Storage
{
    public class AppStateIndexedDBStorage : IndexedDBStorageBase, ImAUserPrivateDataStorage
    {
        #region Construct
        const string databaseName = "AppState";

        public readonly Dexie.Table<object, object> AppStateEntry;

        public AppStateIndexedDBStorage()
            : base(
                  databaseName,
                  new IndexedDBVersionInfo
                  {
                      Version = 1,
                      ConstructDbVersionSchema = ConstructDbVersion1Schema,
                  }
            )
        {
            this.AppStateEntry = database.table(nameof(AppStateEntry));
        }
        #endregion

        private static Dexie.Version.storesConfig ConstructDbVersion1Schema()
        {
            Dexie.Version.storesConfig schema = new Dexie.Version.storesConfig();

            schema[nameof(AppStateEntry)] = "&ID,AsOf,AsOfTicks,ValidFor,ValidForTicks,ExpiresAt,ExpiresAtTicks";

            return schema;
        }

        public Task Purge()
        {
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

            AppStateEntry
                .clear()
                .then(
                    onfulfilled: x => { taskCompletionSource.SetResult(true); return x; },
                    onrejected: x => { taskCompletionSource.SetException(new OperationResultException(OperationResult.Fail($"Error occurred while purging user private data from {databaseName}->{nameof(AppStateEntry)}"))); return null; }
                );

            return taskCompletionSource.Task;
        }
    }
}
