using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime
{
    public interface ImAUserInfoStorageResource
    {
        Task<UserInfo[]> GetUsersByIds(params Guid[] ids);

        Task<UserInfo[]> SearchUsers(UserInfoSearchCriteria searchCriteria);

        Task SaveUser(UserInfo userInfo);
    }
}
