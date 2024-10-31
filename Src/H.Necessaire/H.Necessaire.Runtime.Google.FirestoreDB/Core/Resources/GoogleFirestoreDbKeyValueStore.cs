using H.Necessaire.Runtime.Google.FirestoreDB.Core.Resources.Abstract;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Core.Resources
{
    internal class GoogleFirestoreDbKeyValueStore
        : GoogleFirestoreDbStorageResourceBase<string, GoogleFirestoreDbKeyValueStore.KeyValueEntry, GoogleFirestoreDbKeyValueStore.KeyValueFilter>
        , IKeyValueStorage
    {
        #region Construct
        readonly string storeName = "Default";
        public GoogleFirestoreDbKeyValueStore(string storeName = null)
        {
            this.storeName = storeName.IsEmpty() ? this.storeName : storeName;
        }
        #endregion

        public virtual string StoreName => storeName;

        public async Task<string> Get(string key)
        {
            KeyValueEntry entry = await LoadEntry(key);

            if (entry?.HasExpired() != false)
                return null;

            return entry.Value;
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
            KeyValueEntry entry = await LoadEntry(key);

            if (entry == null)
                return;

            await base.DeleteByID($"{StoreName}-{key}");
        }

        public async Task Remove(string key) => await Zap(key);

        private async Task<KeyValueEntry> LoadEntry(string key)
        {
            return
                (await base.LoadByID($"{StoreName}-{key}"))?.Payload;
        }

        private async Task SetValue(string key, string value, DateTime? validUntil = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                OperationResult.Fail("The key is empty").ThrowOnFail();

            KeyValueEntry entry =
            (await LoadEntry(key))
            ?? new KeyValueEntry
            {
                StoreName = StoreName,
                Key = key,
            };
            entry.Value = value;
            entry.ExpiresAt = validUntil;

            await base.Save(entry);
        }

        public class KeyValueEntry : IStringIdentity
        {
            public string ID => $"{StoreName}-{Key}";
            public string StoreName { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime? ExpiresAt { get; set; }
            public bool HasExpired()
            {
                DateTime expiresAt = ExpiresAt == null ? DateTime.MaxValue : ExpiresAt.Value;
                return DateTime.UtcNow > expiresAt;
            }
        }

        public class KeyValueFilter : IDFilter<string>
        {
            public string[] StoreNames { get; set; }
            public string[] Keys { get; set; }
            public DateTime? FromInclusive { get; set; }
            public DateTime? ToInclusive { get; set; }
        }
    }
}
