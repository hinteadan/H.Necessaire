using static Retyped.dexie;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class HNecessaireIndexedDBStorage : IndexedDBStorageBase
    {
        #region Construct
        const string databaseName = "H.Necessaire";

        public readonly Dexie.Table<object, object> ConsumerIdentity;
        public readonly Dexie.Table<object, object> BffApiSyncRegistry;
        public readonly Dexie.Table<object, object> Log;

        public HNecessaireIndexedDBStorage()
            : base(
                  databaseName,
                  new IndexedDBVersionInfo
                  {
                      Version = 1,
                      ConstructDbVersionSchema = ConstructDbVersion1Schema,
                  },
                  new IndexedDBVersionInfo
                  {
                      Version = 2,
                      ConstructDbVersionSchema = ConstructDbVersion2Schema,
                  },
                  new IndexedDBVersionInfo
                  {
                      Version = 3,
                      ConstructDbVersionSchema = ConstructDbVersion3Schema,
                  }
            )
        {
            this.ConsumerIdentity = database.table(nameof(ConsumerIdentity));
            this.BffApiSyncRegistry = database.table(nameof(BffApiSyncRegistry));
            this.Log = database.table(nameof(Log));
        }
        #endregion

        private static Dexie.Version.storesConfig ConstructDbVersion1Schema()
        {
            Dexie.Version.storesConfig schema = new Dexie.Version.storesConfig();

            schema[nameof(ConsumerIdentity)] = "&ID";

            return schema;
        }

        private static Dexie.Version.storesConfig ConstructDbVersion2Schema()
        {
            Dexie.Version.storesConfig schema = new Dexie.Version.storesConfig();

            schema[nameof(BffApiSyncRegistry)] = "&ID";

            return schema;
        }

        private static Dexie.Version.storesConfig ConstructDbVersion3Schema()
        {
            Dexie.Version.storesConfig schema = new Dexie.Version.storesConfig();

            schema[nameof(Log)] = "&ID, Level, ScopeID, HappenedAt, Method, Component, Application";

            return schema;
        }
    }
}
