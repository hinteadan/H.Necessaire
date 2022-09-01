using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime
{
    public interface ImTheIronManProviderResource
    {
        Task<UserInfo[]> GetIronMenByIds(params Guid[] ids);
        Task<UserInfo[]> SearchIronMen(UserInfoSearchCriteria searchCriteria);
        Task<string> GetPasswordFor(Guid ironManuserID);
    }
}
