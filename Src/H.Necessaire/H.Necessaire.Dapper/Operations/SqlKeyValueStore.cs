using System;
using System.Threading.Tasks;

namespace H.Necessaire.Dapper
{
    public abstract class SqlKeyValueStore : DapperSqlResourceBase, IKeyValueStorage
    {
        #region Construct
        readonly string storeName = "Default";
        public SqlKeyValueStore(string connectionString, string tableName, string storeName = null)
            : base(connectionString, tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException($"Table name is empty", nameof(tableName));

            this.storeName = string.IsNullOrWhiteSpace(storeName) ? this.storeName : storeName;
        }
        #endregion

        public string StoreName => storeName;

        public async Task<string> Get(string key)
        {
            KeyValueSqlEntry sqlEntry = await LoadSqlEntry(key);

            if (sqlEntry?.HasExpired() ?? true)
                return null;

            return sqlEntry.Value;
        }

        public async Task Set(string key, string value, TimeSpan? validFor = null)
        {
            await Set(key, value, validFor == null ? null : (DateTime.UtcNow + validFor.Value) as DateTime?);
        }

        public async Task Set(string key, string value, DateTime? validUntil = null)
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
    }
}
