using H.Necessaire.Runtime.Google.FirestoreDB.Core.Resources.Abstract;
using H.Necessaire.Runtime.Google.FirestoreDB.Security.Resources.Filters;
using H.Necessaire.Runtime.Google.FirestoreDB.Security.Resources.Model;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Security.Resources
{
    internal class GoogleFirestoreDbCachedUserAuthInfoStorageResource
        : GoogleFirestoreDbStorageResourceBase<Guid, UserAuthKey, UserAuthKeyFilter>, ImAUserAuthInfoStorageResource
    {
        static ConcurrentDictionary<Guid, UserAuthKey> cachedKeys = new ConcurrentDictionary<Guid, UserAuthKey>();

        public async Task<string> GetAuthKeyForUser(Guid userID)
        {
            if (cachedKeys.ContainsKey(userID))
                return cachedKeys[userID].Key;

            UserAuthKey key = (await LoadByID(userID)).Payload;

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

            (await Save(userAuthKey)).ThrowOnFail();

            cachedKeys.AddOrUpdate(userID, userAuthKey, (a, b) => userAuthKey);
        }
    }
}
