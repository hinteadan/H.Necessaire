using H.Necessaire.Runtime.Security.Resources;
using H.Necessaire.Runtime.Validation.Engines;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Security.Managers.Concrete
{
    class UserIdentityManager : ImAUserIdentityManager, ImADependency
    {
        #region Construct
        ImAValidationEngine<UserInfo> userInfoValidationEngine;
        ImAUserInfoStorageResource userInfoStorageResource;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            userInfoStorageResource = dependencyProvider.Get<ImAUserInfoStorageResource>();
            userInfoValidationEngine = dependencyProvider.Get<ImAValidationEngine<UserInfo>>();
        }
        #endregion

        public async Task<OperationResult> CreateOrUpdateUser(UserInfo userInfo)
        {
            OperationResult result =
                await userInfoValidationEngine.ValidateEntity(userInfo);
            if (!result.IsSuccessful)
                return result;

            await
                new Func<Task>(async () =>
                {
                    await userInfoStorageResource.SaveUser(userInfo);
                })
                .TryOrFailWithGrace(
                    onFail: ex => result = OperationResult.Fail(ex)
                );

            return result;
        }
    }
}
