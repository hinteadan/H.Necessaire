using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime
{
    public interface ImAUserAuthInfoStorageResource
    {
        Task SaveAuthKeyForUser(Guid userID, string key, params Note[] notes);
        Task<string> GetAuthKeyForUser(Guid userID);
    }
}
