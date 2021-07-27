using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Security.Resources
{
    public interface ImAUserInfoStorageResource
    {
        Task<UserInfo[]> GetUsersByIds(params Guid[] ids);

        Task<UserInfo[]> SearchUsers(DataContracts.UserInfoSearchCriteria searchCriteria);

        Task SaveUser(UserInfo userInfo);
    }
}
