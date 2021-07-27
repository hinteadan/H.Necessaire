using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Security.Resources
{
    public interface ImAUserAuthInfoStorageResource
    {
        Task SaveAuthKeyForUser(Guid userID, string key);
        Task<string> GetAuthKeyForUser(Guid userID);
    }
}
