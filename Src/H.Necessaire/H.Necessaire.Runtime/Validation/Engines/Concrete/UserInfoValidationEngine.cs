﻿using H.Necessaire.Runtime.Validation.Engines.Abstract;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Validation.Engines.Concrete
{
    class UserInfoValidationEngine : ValidationEngineBase<UserInfo>, ImADependency
    {
        ImAUserInfoStorageResource userIdentityStorageResource;
        ImTheIronManProviderResource ironManProviderResource;
        readonly Func<UserInfo, Task<OperationResult>>[] customRules;

        public UserInfoValidationEngine()
        {
            customRules = new Func<UserInfo, Task<OperationResult>>[] {
                userInfo => ValidateEmailAddressFormat(nameof(userInfo.Email), userInfo.Email),
                ValidateUsername
            };
        }

        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            userIdentityStorageResource = dependencyProvider.Get<ImAUserInfoStorageResource>();
            ironManProviderResource = dependencyProvider.Get<ImTheIronManProviderResource>();
        }
        protected override Func<UserInfo, Task<OperationResult>>[] CustomRules => customRules;

        async Task<OperationResult> ValidateUsername(UserInfo userInfo)
        {
            if (string.IsNullOrWhiteSpace(userInfo?.Username))
                return OperationResult.Fail($"Username cannot be empty");

            UserInfo existingIronMan =
                (await ironManProviderResource.SearchIronMen(new UserInfoSearchCriteria
                {
                    Usernames = userInfo.Username.AsArray(),
                }))
                ?.SingleOrDefault()
                ;

            UserInfo existingUser = 
                existingIronMan 
                ?? 
                (
                    (await userIdentityStorageResource.SearchUsers(new UserInfoSearchCriteria
                    {
                        Usernames = userInfo.Username.AsArray()
                    }))
                    ?.SingleOrDefault()
                );

            if (existingUser == null)
                return OperationResult.Win();

            if (existingUser.ID == userInfo.ID)
                return OperationResult.Win();

            return OperationResult.Fail($"The given username ({userInfo.Username}) already exists. Please choose another.");
        }
    }
}
