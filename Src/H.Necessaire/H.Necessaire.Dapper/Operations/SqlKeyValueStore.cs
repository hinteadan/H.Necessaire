using System;
using System.Threading.Tasks;

namespace H.Necessaire.Dapper
{
    public class SqlKeyValueStore : DapperSqlServerResourceBase, IKeyValueStorage
    {
        #region Construct
        readonly string storeName = "Default";
        public SqlKeyValueStore(string storeName = null, string connectionString = null)
            : base(connectionString, tableName: "H.Necessaire.KeyValueStore", databaseName: "H.Necessaire.Core")
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException($"Table name is empty", nameof(tableName));

            this.storeName = string.IsNullOrWhiteSpace(storeName) ? this.storeName : storeName;
        }

        protected override async Task<SqlMigration[]> GetAllMigrations()
        {
            return new SqlMigration
            {
                VersionNumber = new VersionNumber(1, 0),
                ResourceIdentifier = "H.Necessaire.KeyValueStore",
                SqlCommand = await ReadSqlFromEmbedResourceSql("Create_KeyValueStore_Table.sql"),
            }
            .AsArray();
        }
        #endregion

        public virtual string StoreName => storeName;

        public async Task<string> Get(string key)
        {
            KeyValueSqlEntry sqlEntry = await LoadSqlEntry(key);

            if (sqlEntry?.HasExpired() ?? true)
                return null;

            return sqlEntry.Value;
        }

        public async Task Set(string key, string value)
        {
            await SetValue(key, value, null);
        }

        public async Task SetFor(string key, string value, TimeSpan validFor)
        {
            await SetValue(key, value, DateTime.UtcNow + validFor);
        }

        public async Task SetUntil(string key, string value, DateTime validUntil)
        {
            await SetValue(key, value, validUntil);
        }

        public async Task Zap(string key)
        {
            KeyValueSqlEntry sqlEntry = await LoadSqlEntry(key);

            if (sqlEntry == null)
                return;

            await DeleteEntity<KeyValueSqlEntry>(sqlEntry.ID);
        }

        public async Task Remove(string key) => await Zap(key);

        private async Task<KeyValueSqlEntry> LoadSqlEntry(string key)
        {
            return
                await LoadEntityByCustomCriteria<KeyValueSqlEntry>(
                    new[] {
                        new SqlFilterCriteria(nameof(StoreName)),
                        new SqlFilterCriteria(nameof(key))
                    },
                    new { StoreName, key }
                );
        }

        private async Task SetValue(string key, string value, DateTime? validUntil = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                OperationResult.Fail("The key is empty").ThrowOnFail();

            KeyValueSqlEntry entry =
            (await LoadSqlEntry(key))
            ?? new KeyValueSqlEntry
            {
                StoreName = StoreName,
                Key = key,
            };
            entry.Value = value;
            entry.ExpiresAtTicks = validUntil?.Ticks;

            await SaveEntity(entry);
        }
    }
}
