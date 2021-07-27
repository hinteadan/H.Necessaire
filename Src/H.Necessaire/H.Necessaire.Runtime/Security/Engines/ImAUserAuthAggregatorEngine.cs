using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Security.Engines
{
    public interface ImAUserAuthAggregatorEngine
    {
        Task<AuthInfo> AggregateBrandNewAuthInfoForUser(Guid userID);
        Task<OperationResult<SecurityContext>> AggregateSecurityContextFromAccessToken(string accessToken);
        Task<OperationResult<SecurityContext>> AggregateRefreshedSecurityContextFromExpiredAccessToken(string expiredAccessToken, string refreshToken);
    }
}
