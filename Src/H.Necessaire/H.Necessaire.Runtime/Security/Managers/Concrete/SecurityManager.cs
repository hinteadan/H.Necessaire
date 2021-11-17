using H.Necessaire.Runtime.Security.Engines;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Security.Managers.Concrete
{
    class SecurityManager : ImASecurityManager, ImADependency
    {
        #region Construct
        ImAUserInfoStorageResource userInfoStorageResource;
        ImAUserCredentialsStorageResource userCredentialsStorageResource;
        ImAHasherEngine hasher;
        ImAUserAuthAggregatorEngine userAuthAggregatorEngine;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            hasher = dependencyProvider.Get<HasherFactory>().GetDefaultHasher();
            userCredentialsStorageResource = dependencyProvider.Get<ImAUserCredentialsStorageResource>();
            userInfoStorageResource = dependencyProvider.Get<ImAUserInfoStorageResource>();
            userAuthAggregatorEngine = dependencyProvider.Get<ImAUserAuthAggregatorEngine>();
        }
        #endregion

        public Task<OperationResult<SecurityContext>> AuthenticateAccessToken(string token)
        {
            return userAuthAggregatorEngine.AggregateSecurityContextFromAccessToken(token);
        }

        public Task<OperationResult<SecurityContext>> RefreshAccessToken(string expiredToken, string refreshToken)
        {
            return userAuthAggregatorEngine.AggregateRefreshedSecurityContextFromExpiredAccessToken(expiredToken, refreshToken);
        }

        public async Task<OperationResult<SecurityContext>> AuthenticateCredentials(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return OperationResult.Fail("Invalid Credentials").WithoutPayload<SecurityContext>();

            UserInfo user
                = (await userInfoStorageResource.SearchUsers(new UserInfoSearchCriteria { Usernames = username?.AsArray() }))?.SingleOrDefault();

            if (user == null)
                return OperationResult.Fail("Invalid Credentials").WithoutPayload<SecurityContext>();

            string passwordHash = await userCredentialsStorageResource.GetPasswordFor(user.ID);

            if (string.IsNullOrWhiteSpace(passwordHash))
                return OperationResult.Fail("Invalid Credentials").WithoutPayload<SecurityContext>();

            OperationResult matchResult = await hasher.VerifyMatch(password, SecuredHash.TryParse(passwordHash).ThrowOnFailOrReturn());

            if (!matchResult.IsSuccessful)
                return OperationResult.Fail("Invalid Credentials").WithoutPayload<SecurityContext>();

            return OperationResult.Win().WithPayload(new SecurityContext
            {
                User = user,
                Auth = await userAuthAggregatorEngine.AggregateBrandNewAuthInfoForUser(user.ID),
                //TODO: Populate Roles when implemented
            });
        }

        public async Task<OperationResult> SetPasswordForUser(Guid userID, string newPassword)
        {
            UserInfo userInfo = (await userInfoStorageResource.GetUsersByIds(userID))?.SingleOrDefault();
            if (userInfo == null)
                return OperationResult.Fail($"The user ({userID}) doesn't exist");

            OperationResult result = OperationResult.Win();

            await
                new Func<Task>(
                    async () => await userCredentialsStorageResource.SetPasswordFor(userInfo, (await hasher.Hash(newPassword)).ToString())
                )
                .TryOrFailWithGrace(
                    onFail: ex => result = OperationResult.Fail(ex)
                );

            return result;
        }
    }
}
