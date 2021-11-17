using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Resources.Concrete
{
    public class KeyValueFileSystemStorageResource : JsonCachedFileSystemStorageServiceBase<string, KeyValueFileSystemStorageResource.KeyValueEntry, KeyValueFileSystemStorageResource.KeyValueFilter>, IKeyValueStorage
    {
        #region Construct
        readonly string storeName = "Default";

        public KeyValueFileSystemStorageResource(string storeName = null)
        {
            this.storeName = string.IsNullOrWhiteSpace(storeName) ? this.storeName : storeName;
        }

        protected override IEnumerable<KeyValueEntry> ApplyFilter(IEnumerable<KeyValueEntry> stream, KeyValueFilter filter)
        {
            IEnumerable<KeyValueEntry> result = stream;

            if (filter?.IDs?.Any() ?? false)
            {
                result = result.Where(x => x.ID.In(filter.IDs));
            }

            if (filter?.StoreNames?.Any() ?? false)
            {
                result = result.Where(x => x.StoreName.In(filter.StoreNames));
            }

            if (filter?.Keys?.Any() ?? false)
            {
                result = result.Where(x => x.Key.In(filter.Keys));
            }

            if (filter?.FromInclusive != null)
            {
                result = result.Where(x => x.CreatedAt >= filter.FromInclusive.Value);
            }

            if (filter?.ToInclusive != null)
            {
                result = result.Where(x => x.CreatedAt <= filter.ToInclusive.Value);
            }

            result = result.ApplySortAndPageFilterIfAny(filter);

            return result;
        }

        public virtual string StoreName => storeName;
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

            await base.DeleteByID($"{StoreName}-{key}");
        }

        public async Task Remove(string key) => await Zap(key);

        private async Task<KeyValueEntry> LoadEntry(string key)
        {
            return
                (await base.LoadByID($"{StoreName}-{key}")).ThrowOnFailOrReturn();
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

        public class KeyValueFilter : SortFilterBase, IPageFilter
        {
            protected override string[] ValidSortNames { get; } = new string[] { nameof(KeyValueEntry.ID) };

            public string[] IDs { get; set; }
            public string[] StoreNames { get; set; }
            public string[] Keys { get; set; }
            public DateTime? FromInclusive { get; set; }
            public DateTime? ToInclusive { get; set; }

            public PageFilter PageFilter { get; set; }
        }
    }
}
