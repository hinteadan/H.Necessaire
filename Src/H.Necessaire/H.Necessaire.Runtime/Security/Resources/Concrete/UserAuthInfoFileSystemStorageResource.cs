using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Security.Resources.Concrete
{
    internal class UserAuthInfoFileSystemStorageResource
        : JsonCachedFileSystemStorageServiceBase<Guid, UserAuthInfoFileSystemStorageResource.UserAuthKey, UserAuthInfoFileSystemStorageResource.UserAuthKeyFilter>
        , ImAUserAuthInfoStorageResource
    {
        static ConcurrentDictionary<Guid, UserAuthKey> cachedKeys = new ConcurrentDictionary<Guid, UserAuthKey>();

        public async Task<string> GetAuthKeyForUser(Guid userID)
        {
            if (cachedKeys.ContainsKey(userID))
                return cachedKeys[userID].Key;

            UserAuthKey key = (await LoadByID(userID)).ThrowOnFailOrReturn();

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
                Notes = notes,
            };

            await Save(userAuthKey);

            cachedKeys.AddOrUpdate(userID, userAuthKey, (a, b) => userAuthKey);
        }

        protected override IEnumerable<UserAuthKey> ApplyFilter(IEnumerable<UserAuthKey> stream, UserAuthKeyFilter filter)
        {
            IEnumerable<UserAuthKey> result = stream;

            if (filter?.IDs?.Any() ?? false)
            {
                result = result.Where(x => x.ID.In(filter.IDs));
            }

            if (filter?.Keys?.Any() ?? false)
            {
                result = result.Where(x => x.Key.In(filter.Keys));
            }

            result = result.ApplySortAndPageFilterIfAny(filter);

            return result;
        }

        public class UserAuthKey : IGuidIdentity
        {
            public Guid ID { get; set; } = Guid.NewGuid();
            public string Key { get; set; }
            public Note[] Notes { get; set; }
        }

        public class UserAuthKeyFilter : SortFilterBase, IPageFilter
        {
            public Guid[] IDs { get; set; }

            public string[] Keys { get; set; }

            public PageFilter PageFilter { get; set; }

            protected override string[] ValidSortNames { get; } = new string[] { nameof(UserAuthKey.ID), nameof(UserAuthKey.Key) };
        }
    }
}
