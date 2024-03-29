﻿using H.Necessaire.RavenDB;
using H.Necessaire.Runtime.RavenDB.Security.Resources.Filters;
using H.Necessaire.Runtime.RavenDB.Security.Resources.Indexes;
using H.Necessaire.Runtime.RavenDB.Security.Resources.Model;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.RavenDB.Security.Resources
{
    class RavenDbUserCredentialsStorageResource : RavenDbStorageResourceBase<Guid, UserCredentials, UserCredentialsFilter, UserCredentialsFilterIndex>, ImAUserCredentialsStorageResource
    {
        #region Construct        
        protected override string DatabaseName => $"{base.DatabaseName}.UserIdentity";
        protected override Guid GetIdFor(UserCredentials item) => item.ID;

        private static Lazy<UserCredentialsFilterIndex> userCredentialsFilterIndex = new Lazy<UserCredentialsFilterIndex>(() => new UserCredentialsFilterIndex());

        protected override async Task EnsureIndexes()
        {
            await base.EnsureIndexes();
            await EnsureIndex(() => userCredentialsFilterIndex.Value);
        }

        protected override IRavenQueryable<UserCredentials> ApplyFilter(IRavenQueryable<UserCredentials> query, UserCredentialsFilter filter)
        {
            if (filter == null)
                return query;

            query = query.Where(x => x.ID != Guid.Empty);

            if (filter.IDs?.Any() ?? false)
                query = query.Intersect().Where(x => RavenQueryableExtensions.In(x.ID, filter.IDs));

            string[] validPasswordHashVersions = filter.PasswordHashVersions?.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            if (validPasswordHashVersions?.Any() ?? false)
            {
                query = query.Intersect().Search(x => x.Password, validPasswordHashVersions);
            }

            if (filter.UserInfoIDs?.Any() ?? false)
                query = query.Intersect().Where(x => RavenQueryableExtensions.In(x.UserInfo, filter.UserInfoIDs));

            return query;
        }
        #endregion

        public async Task SetPasswordFor(UserInfo userInfo, string password)
        {
            if (userInfo == null)
                throw new InvalidOperationException("The UserInfo must be provided");

            UserCredentials userCredentials
                = (await Search(new UserCredentialsFilter { UserInfoIDs = new[] { userInfo.ID } }))?.SingleOrDefault()
                ?? new UserCredentials { UserInfo = userInfo.ID };

            userCredentials.Password = password;

            await Save(userCredentials);
        }

        public async Task<string> GetPasswordFor(Guid userID)
        {
            return
                (await Search(new UserCredentialsFilter { UserInfoIDs = new[] { userID } }))
                ?.SingleOrDefault()
                ?.Password;
        }
    }
}
