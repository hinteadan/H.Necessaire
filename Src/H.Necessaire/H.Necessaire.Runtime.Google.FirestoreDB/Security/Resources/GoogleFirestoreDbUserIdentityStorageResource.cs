﻿using H.Necessaire.Runtime.Google.FirestoreDB.Core.Resources.Abstract;
using H.Necessaire.Runtime.Google.FirestoreDB.Security.Resources.Filters;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Security.Resources
{
    internal class GoogleFirestoreDbUserIdentityStorageResource
        : GoogleFirestoreDbStorageResourceBase<Guid, UserInfo, UserIdentityFilter>, ImAUserInfoStorageResource
    {
        public async Task<UserInfo[]> GetUsersByIds(params Guid[] ids)
        {
            return (await LoadPage(new UserIdentityFilter { IDs = ids })).ThrowOnFailOrReturn().Content;
        }

        public async Task SaveUser(UserInfo userInfo)
        {
            (await Save(userInfo)).ThrowOnFail();
        }

        public async Task<UserInfo[]> SearchUsers(UserInfoSearchCriteria searchCriteria)
        {
            return (await LoadPage(new UserIdentityFilter { IDs = searchCriteria?.IDs, Usernames = searchCriteria?.Usernames })).ThrowOnFailOrReturn().Content;
        }
    }
}
