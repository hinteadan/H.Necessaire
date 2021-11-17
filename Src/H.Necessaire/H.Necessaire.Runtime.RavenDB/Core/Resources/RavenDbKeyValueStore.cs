using H.Necessaire.RavenDB;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.RavenDB.Core.Resources
{
    public class RavenDbKeyValueStore : RavenDbStorageResourceBase<string, RavenDbKeyValueStore.KeyValueEntry, RavenDbKeyValueStore.KeyValueFilter, RavenDbKeyValueStore.KeyValueFilterIndex>, IKeyValueStorage
    {
        #region Construct
        readonly string storeName = "Default";

        public RavenDbKeyValueStore(string storeName = null)
        {
            this.storeName = string.IsNullOrWhiteSpace(storeName) ? this.storeName : storeName;
        }

        protected override string DatabaseName { get; } = "H.Necessaire.Core.KeyValueStore";

        protected override string GetIdFor(KeyValueEntry item) => item.ID;

        private static Lazy<KeyValueFilterIndex> keyValueFilterIndex = new Lazy<KeyValueFilterIndex>(() => new KeyValueFilterIndex());

        protected override async Task EnsureIndexes()
        {
            await base.EnsureIndexes();
            await EnsureIndex(() => keyValueFilterIndex.Value);
        }

        public virtual string StoreName => storeName;

        protected override IRavenQueryable<KeyValueEntry> ApplyFilter(IRavenQueryable<KeyValueEntry> query, KeyValueFilter filter)
        {
            if (filter == null)
                return query;

            query = query.Where(x => x.ID != string.Empty);

            if (filter?.IDs?.Any() ?? false)
                query = query.Intersect().Where(x => RavenQueryableExtensions.In(x.ID, filter.IDs));

            if (filter?.StoreNames?.Any() ?? false)
                query = query.Intersect().Where(x => RavenQueryableExtensions.In(x.StoreName, filter.StoreNames));

            if (filter?.Keys?.Any() ?? false)
                query = query.Intersect().Where(x => RavenQueryableExtensions.In(x.Key, filter.Keys));

            if (filter?.FromInclusive != null)
                query = query.Intersect().Where(x => x.CreatedAt >= filter.FromInclusive.Value);

            if (filter?.ToInclusive != null)
                query = query.Intersect().Where(x => x.CreatedAt <= filter.ToInclusive.Value);

            return query;
        }
        #endregion

        public async Task<string> Get(string key)
        {
            KeyValueEntry entry = await LoadEntry(key);

            if (entry?.HasExpired() ?? true)
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

            await base.Delete($"{StoreName}-{key}");
        }

        public async Task Remove(string key) => await Zap(key);

        private async Task<KeyValueEntry> LoadEntry(string key)
        {
            return
                await base.Load($"{StoreName}-{key}");
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

        public class KeyValueFilter
        {
            public string[] IDs { get; set; }
            public string[] StoreNames { get; set; }
            public string[] Keys { get; set; }
            public DateTime? FromInclusive { get; set; }
            public DateTime? ToInclusive { get; set; }
        }

        public class KeyValueFilterIndex : AbstractIndexCreationTask<KeyValueEntry>
        {
            public KeyValueFilterIndex()
            {
                Map = docs => docs.Select(doc =>
                    new
                    {
                        doc.ID,
                        doc.StoreName,
                        doc.Key,
                        doc.CreatedAt,
                    }
                );
            }
        }
    }
}
