using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime
{
    public interface ImASecurityManager
    {
        Task<OperationResult<SecurityContext>> AuthenticateCredentials(string username, string password);
        Task<OperationResult<SecurityContext>> AuthenticateAccessToken(string token);
        Task<OperationResult<SecurityContext>> RefreshAccessToken(string expiredAccessToken, string refreshToken);
        Task<OperationResult> SetPasswordForUser(Guid userID, string newPassword);
    }
}
