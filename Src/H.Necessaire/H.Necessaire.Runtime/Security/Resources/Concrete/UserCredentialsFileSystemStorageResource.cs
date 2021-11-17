using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Security.Resources.Concrete
{
    internal class UserCredentialsFileSystemStorageResource
        : JsonCachedFileSystemStorageServiceBase<Guid, UserCredentialsFileSystemStorageResource.UserCredentials, UserCredentialsFileSystemStorageResource.UserCredentialsFilter>
        , ImAUserCredentialsStorageResource
    {
        public Task<string> GetPasswordFor(Guid userID) => LoadByID(userID).ContinueWith(x => x.Result.ThrowOnFailOrReturn()?.Password);

        public Task SetPasswordFor(UserInfo userInfo, string password) => Save(new UserCredentials { ID = userInfo.ID, Password = password, UserInfoID = userInfo?.ID ?? Guid.Empty });

        protected override IEnumerable<UserCredentials> ApplyFilter(IEnumerable<UserCredentials> stream, UserCredentialsFilter filter)
        {
            IEnumerable<UserCredentials> result = stream;

            if (filter?.IDs?.Any() ?? false)
            {
                result = result.Where(x => x.ID.In(filter.IDs));
            }

            if (filter?.PasswordHashVersions?.Any() ?? false)
            {
                result = result.Where(x => filter.PasswordHashVersions.Any(v => x.Password.Contains(v)));
            }

            if (filter?.UserInfoIDs?.Any() ?? false)
            {
                result = result.Where(x => x.UserInfoID.In(filter.UserInfoIDs));
            }

            result = result.ApplySortAndPageFilterIfAny(filter);

            return result;
        }

        public class UserCredentials : IGuidIdentity
        {
            public Guid ID { get; set; } = Guid.NewGuid();
            public string Password { get; set; }
            public Guid UserInfoID { get; set; }
        }

        public class UserCredentialsFilter : SortFilterBase, IPageFilter
        {
            public Guid[] IDs { get; set; }
            public string[] PasswordHashVersions { get; set; }
            public Guid[] UserInfoIDs { get; set; }

            public PageFilter PageFilter { get; set; }

            protected override string[] ValidSortNames { get; } = new string[] { nameof(UserCredentials.ID) };
        }
    }
}
