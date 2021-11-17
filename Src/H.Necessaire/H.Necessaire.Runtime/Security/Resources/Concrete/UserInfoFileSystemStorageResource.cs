using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Security.Resources.Concrete
{
    internal class UserInfoFileSystemStorageResource
        : JsonCachedFileSystemStorageServiceBase<Guid, UserInfo, UserInfoFileSystemStorageResource.UserInfoSearchFilter>
        , ImAUserInfoStorageResource
    {
        public Task<UserInfo[]> GetUsersByIds(params Guid[] ids) => LoadByIDs(ids).ContinueWith(x => x.Result.Where(r => r.IsSuccessful && r.Payload != null).Select(r => r.Payload).ToArray());

        public Task SaveUser(UserInfo userInfo) => Save(userInfo).ContinueWith(x => x.Result.ThrowOnFail());

        public Task<UserInfo[]> SearchUsers(UserInfoSearchCriteria searchCriteria)
            => Stream(new UserInfoSearchFilter { IDs = searchCriteria.IDs, Usernames = searchCriteria.Usernames })
            .ContinueWith(x =>
            {
                using (IDisposableEnumerable<UserInfo> userInfos = x.Result.ThrowOnFailOrReturn())
                {
                    return userInfos.ToArray();
                }
            });

        protected override IEnumerable<UserInfo> ApplyFilter(IEnumerable<UserInfo> stream, UserInfoSearchFilter filter)
        {
            IEnumerable<UserInfo> result = stream;

            if (filter?.IDs?.Any() ?? false)
            {
                result = result.Where(x => x.ID.In(filter.IDs));
            }

            if (filter?.Usernames?.Any() ?? false)
            {
                result = result.Where(x => x.Username.In(filter.Usernames, (a, b) => string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase)));
            }

            result = result.ApplySortAndPageFilterIfAny(filter);

            return result;
        }

        public class UserInfoSearchFilter : UserInfoSearchCriteria, IPageFilter, ISortFilter
        {
            static readonly string[] validateSortFilters = new string[] { nameof(UserInfo.Username), nameof(UserInfo.DisplayName), nameof(UserInfo.Email), nameof(UserInfo.ID), nameof(UserInfo.IDTag) };

            public PageFilter PageFilter { get; set; }

            public SortFilter[] SortFilters { get; set; }

            public OperationResult ValidateSortFilters() => this.ValidateSortFilters(validateSortFilters);
        }
    }
}
