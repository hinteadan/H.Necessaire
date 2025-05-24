using H.Necessaire.Dapper;
using H.Necessaire.Serialization;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.SqlServer.Security.Resources
{
    internal partial class SqlServerCachedUserAuthInfoStorageResource : DapperSqlServerResourceBase, ImAUserAuthInfoStorageResource
    {
        #region Construct
        static ConcurrentDictionary<Guid, UserAuthKey> cachedKeys = new ConcurrentDictionary<Guid, UserAuthKey>();

        public SqlServerCachedUserAuthInfoStorageResource()
            : base(connectionString: null, tableName: nameof(UserAuthKey), databaseName: "H.Necessaire.Core")
        { }

        protected override Task<SqlMigration[]> GetAllMigrations() => sqlMigrations.AsTask();

        protected override bool IsCoreDatabase() => true;
        #endregion

        public async Task<string> GetAuthKeyForUser(Guid userID)
        {
            if (cachedKeys.ContainsKey(userID))
                return cachedKeys[userID].Key;

            UserAuthKey key = (await base.LoadEntityByID<UserAuthKey>(userID));

            if (key == null)
                return null;

            cachedKeys.AddOrUpdate(userID, key, (a, b) => key);

            return key.Key;
        }

        public async Task SaveAuthKeyForUser(Guid userID, string key, params Note[] notes)
        {
            UserAuthKey userAuthKey = new UserAuthKey
            {
                ID = userID,
                Key = key,
                NotesJson = notes?.ToJsonArray(),
            };

            await base.SaveEntity(userAuthKey);

            cachedKeys.AddOrUpdate(userID, userAuthKey, (a, b) => userAuthKey);
        }

        class UserAuthKey : SqlEntryBase, IGuidIdentity
        {
            public Guid ID { get; set; } = Guid.NewGuid();
            public string Key { get; set; }
            public string NotesJson { get; set; }
        }
    }
}
