using H.Necessaire.Runtime.Google.FirestoreDB.Core.Resources.Abstract;
using H.Necessaire.Runtime.Google.FirestoreDB.Security.Resources.Filters;
using H.Necessaire.Runtime.Google.FirestoreDB.Security.Resources.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Security.Resources
{
    internal class GoogleFirestoreDbUserCredentialsStorageResource
        : GoogleFirestoreDbStorageResourceBase<Guid, UserCredentials, UserCredentialsFilter>, ImAUserCredentialsStorageResource
    {
        public async Task<string> GetPasswordFor(Guid userID)
        {
            return
                (await LoadPage(new UserCredentialsFilter { UserInfoIDs = new[] { userID } })).ThrowOnFailOrReturn().Content
                ?.SingleOrDefault()
                ?.Password;
        }

        public async Task SetPasswordFor(UserInfo userInfo, string password)
        {
            if (userInfo == null)
                throw new InvalidOperationException("The UserInfo must be provided");

            UserCredentials userCredentials
                = (await LoadPage(new UserCredentialsFilter { UserInfoIDs = new[] { userInfo.ID } })).ThrowOnFailOrReturn().Content?.SingleOrDefault()
                ?? new UserCredentials { UserInfo = userInfo.ID };

            userCredentials.Password = password;

            await Save(userCredentials);
        }
    }
}
