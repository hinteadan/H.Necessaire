using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Security.Resources
{
    public interface ImAUserCredentialsStorageResource
    {
        Task SetPasswordFor(UserInfo userInfo, string password);
        Task<string> GetPasswordFor(Guid userID);
    }
}
