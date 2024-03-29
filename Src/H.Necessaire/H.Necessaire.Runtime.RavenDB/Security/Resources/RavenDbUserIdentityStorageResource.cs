﻿using H.Necessaire.RavenDB;
using H.Necessaire.Runtime.RavenDB.Security.Resources.Filters;
using H.Necessaire.Runtime.RavenDB.Security.Resources.Indexes;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.RavenDB.Security.Resources
{
    class RavenDbUserIdentityStorageResource : RavenDbStorageResourceBase<Guid, UserInfo, UserIdentityFilter, UserIdentityFilterIndex>, ImAUserInfoStorageResource
    {
        #region Construct
        protected override string DatabaseName => $"{base.DatabaseName}.UserIdentity";
        protected override Guid GetIdFor(UserInfo item) => item.ID;

        private static Lazy<UserIdentityFilterIndex> userIdentityFilterIndex = new Lazy<UserIdentityFilterIndex>(() => new UserIdentityFilterIndex());

        protected override async Task EnsureIndexes()
        {
            await base.EnsureIndexes();
            await EnsureIndex(() => userIdentityFilterIndex.Value);
        }

        protected override IRavenQueryable<UserInfo> ApplyFilter(IRavenQueryable<UserInfo> query, UserIdentityFilter filter)
        {
            if (filter == null)
                return query;

            query = query.Where(x => x.ID != Guid.Empty);

            if (filter.IDs?.Any() ?? false)
                query = query.Intersect().Where(x => RavenQueryableExtensions.In(x.ID, filter.IDs));

            string[] validUsernames = filter.Usernames?.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            if (validUsernames?.Any() ?? false)
                query = query.Intersect().Where(x => RavenQueryableExtensions.In(x.Username, validUsernames), exact: false);

            return query;
        }
        #endregion

        public Task<UserInfo[]> GetUsersByIds(params Guid[] ids)
        {
            return Search(new UserIdentityFilter { IDs = ids });
        }

        public Task<UserInfo[]> SearchUsers(UserInfoSearchCriteria searchCriteria)
        {
            return Search(new UserIdentityFilter { IDs = searchCriteria?.IDs, Usernames = searchCriteria?.Usernames });
        }

        public async Task SaveUser(UserInfo userInfo)
        {
            await Save(userInfo);
        }
    }
}
