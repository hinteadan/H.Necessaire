using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Security.Managers
{
    public interface ImAUserIdentityManager
    {
        Task<OperationResult> CreateOrUpdateUser(UserInfo userInfo);
    }
}
