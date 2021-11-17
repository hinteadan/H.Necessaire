using H.Necessaire.RavenDB;
using H.Necessaire.Runtime.RavenDB.Security.Resources.Filters;
using H.Necessaire.Runtime.RavenDB.Security.Resources.Indexes;
using H.Necessaire.Runtime.RavenDB.Security.Resources.Model;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.RavenDB.Security.Resources
{
    class RavenDbCachedUserAuthInfoStorageResource : RavenDbStorageResourceBase<Guid, UserAuthKey, UserAuthKeyFilter, UserAuthKeyFilterIndex>, ImAUserAuthInfoStorageResource
    {
        #region Construct
        public const string dbNameUserAuthentication = "UserAuthentication";

        static ConcurrentDictionary<Guid, UserAuthKey> cachedKeys = new ConcurrentDictionary<Guid, UserAuthKey>();

        protected override string DatabaseName => dbNameUserAuthentication;
        protected override Guid GetIdFor(UserAuthKey item) => item.ID;

        private static Lazy<UserAuthKeyFilterIndex> userAuthKeyFilterIndex = new Lazy<UserAuthKeyFilterIndex>(() => new UserAuthKeyFilterIndex());

        protected override async Task EnsureIndexes()
        {
            await base.EnsureIndexes();
            await EnsureIndex(() => userAuthKeyFilterIndex.Value);
        }

        protected override IRavenQueryable<UserAuthKey> ApplyFilter(IRavenQueryable<UserAuthKey> query, UserAuthKeyFilter filter)
        {
            if (filter == null)
                return query;

            query = query.Where(x => x.ID != Guid.Empty);

            if (filter.IDs?.Any() ?? false)
                query = query.Intersect().Where(x => RavenQueryableExtensions.In(x.ID, filter.IDs));

            return query;
        }
        #endregion

        public async Task<string> GetAuthKeyForUser(Guid userID)
        {
            if (cachedKeys.ContainsKey(userID))
                return cachedKeys[userID].Key;

            UserAuthKey key = await Load(userID);

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
    }
}
