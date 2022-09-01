using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Security.Engines.Concrete
{
    class UserAuthAggregatorEngine : ImAUserAuthAggregatorEngine, ImADependency
    {
        #region Construct
        static readonly TimeSpan authTokenValidFor = TimeSpan.FromHours(1);
        static readonly TimeSpan refreshTokenValidFor = TimeSpan.FromDays(14);

        ImAHasherEngine hasher;
        ImAUserAuthInfoStorageResource userAuthInfoStorageResource;
        ImAUserInfoStorageResource userInfoStorageResource;
        ImTheIronManProviderResource ironManProviderResource;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            hasher = dependencyProvider.Get<HasherFactory>().GetDefaultHasher();
            userAuthInfoStorageResource = dependencyProvider.Get<ImAUserAuthInfoStorageResource>();
            userInfoStorageResource = dependencyProvider.Get<ImAUserInfoStorageResource>();
            ironManProviderResource = dependencyProvider.Get<ImTheIronManProviderResource>();
        }
        #endregion

        public async Task<AuthInfo> AggregateBrandNewAuthInfoForUser(Guid userID)
        {
            UserInfo ironMan = (await ironManProviderResource.GetIronMenByIds(userID))?.SingleOrDefault();
            UserInfo userInfo = ironMan ?? (await userInfoStorageResource.GetUsersByIds(userID))?.SingleOrDefault();
            if (userInfo == null)
                OperationResult.Fail($"The user ({userID}) doesn't exist").ThrowOnFail();

            DateTime createdAt = DateTime.UtcNow;

            JsonWebToken<AccessTokenJwtPayload> accessTokenJWT = await ConstructUnsignedAccessTokenJWT(userInfo, createdAt);
            JsonWebToken<RefreshTokenJwtPayload> refreshTokenJWT = await ConstructUnsignedRefreshTokenJWT(accessTokenJWT.Payload);

            SecuredHash securedHashForAccessTokenJWT = await hasher.Hash(accessTokenJWT.ToStringUnsigned());
            SecuredHash securedHashForRefreshTokenJWT = await hasher.Hash(refreshTokenJWT.ToStringUnsigned(), securedHashForAccessTokenJWT.Key);

            await userAuthInfoStorageResource.SaveAuthKeyForUser(userID, securedHashForAccessTokenJWT.Key, securedHashForAccessTokenJWT.Notes?.Where(x => x.ID.In(RS512Hasher.KnownNoteIdPrivateKey, RS512Hasher.KnownNoteIdPublicKey)).ToArray());

            accessTokenJWT.Signature = securedHashForAccessTokenJWT.Hash;
            refreshTokenJWT.Signature = securedHashForRefreshTokenJWT.Hash;

            AuthInfo result =
                new AuthInfo
                {
                    AccessToken = accessTokenJWT.ToStringSigned(),
                    RefreshToken = refreshTokenJWT.ToStringSigned(),
                    AccessTokenGeneratedAt = createdAt,
                    AccessTokenValidFor = authTokenValidFor,
                };

            return result;
        }

        private Task<JsonWebToken<AccessTokenJwtPayload>> ConstructUnsignedAccessTokenJWT(UserInfo userInfo, DateTime createdAt)
        {
            return
                new JsonWebToken<AccessTokenJwtPayload>
                {
                    Payload = new AccessTokenJwtPayload
                    {
                        UserID = userInfo.ID,
                        ValidUntil = createdAt + authTokenValidFor,
                        ValidFrom = createdAt,
                        IssuedAt = createdAt,
                        UserInfo = userInfo,
                    },
                }
                .AsTask();
        }

        private Task<JsonWebToken<RefreshTokenJwtPayload>> ConstructUnsignedRefreshTokenJWT(AccessTokenJwtPayload accessTokenPayload)
        {
            return
                new JsonWebToken<RefreshTokenJwtPayload>
                {
                    Payload = new RefreshTokenJwtPayload
                    {
                        ValidUntil = accessTokenPayload.ValidUntil + refreshTokenValidFor,
                        ValidFrom = accessTokenPayload.ValidFrom,
                        IssuedAt = accessTokenPayload.IssuedAt,
                        AccessTokenID = accessTokenPayload.ID,
                        UserID = accessTokenPayload.UserID,
                    },
                }
                .AsTask();
        }

        public async Task<OperationResult<SecurityContext>> AggregateSecurityContextFromAccessToken(string accessToken)
        {
            OperationResult<SecurityContext> result = null;

            await
                new Func<Task>(async () =>
                {
                    JsonWebToken<AccessTokenJwtPayload> accessTokenJWT = accessToken.TryParseJsonWebToken<AccessTokenJwtPayload>().ThrowOnFailOrReturn();

                    string userAuthKey = await userAuthInfoStorageResource.GetAuthKeyForUser(accessTokenJWT.Payload.UserID);

                    OperationResult tokenSignatureValidationResult = await hasher.VerifyMatch(accessTokenJWT.ToStringUnsigned(), new SecuredHash { Hash = accessTokenJWT.Signature, Key = userAuthKey });

                    if (!tokenSignatureValidationResult.IsSuccessful)
                    {
                        result = OperationResult.Fail($"The access token signature is invalid which probably means that the token has been tempered with.", tokenSignatureValidationResult.FlattenReasons()).WithoutPayload<SecurityContext>();
                        return;
                    }

                    accessTokenJWT.ValidateTiming().ThrowOnFail();

                    result = OperationResult.Win().WithPayload(new SecurityContext
                    {
                        User = accessTokenJWT.Payload.UserInfo,
                        Auth = new AuthInfo
                        {
                            AccessToken = accessToken,
                            AccessTokenGeneratedAt = accessTokenJWT.Payload.IssuedAt ?? DateTime.UtcNow,
                            AccessTokenType = WellKnownAccessTokenType.Bearer,
                            AccessTokenValidFor
                                = accessTokenJWT.Payload.ValidUntil == null
                                ? default(TimeSpan)
                                : accessTokenJWT.Payload.ValidUntil.Value - (accessTokenJWT.Payload.IssuedAt ?? DateTime.UtcNow),
                        },
                        Roles = accessTokenJWT.Payload.Roles ?? new Role[0],
                    });
                })
                .TryOrFailWithGrace(
                    onFail: ex => result = OperationResult.Fail(ex, $"An error occured while validating the provided access token. See comments for details. Access Token: {accessToken}").WithoutPayload<SecurityContext>()
                );

            return result;
        }

        public async Task<OperationResult<SecurityContext>> AggregateRefreshedSecurityContextFromExpiredAccessToken(string expiredAccessToken, string refreshToken)
        {
            OperationResult<SecurityContext> result = null;

            await
                new Func<Task>(async () =>
                {
                    JsonWebToken<RefreshTokenJwtPayload> refreshTokenJWT = refreshToken.TryParseJsonWebToken<RefreshTokenJwtPayload>().ThrowOnFailOrReturn();

                    refreshTokenJWT.ValidateTiming().ThrowOnFail();

                    JsonWebToken<AccessTokenJwtPayload> expiredAccessTokenJWT = expiredAccessToken.TryParseJsonWebToken<AccessTokenJwtPayload>().ThrowOnFailOrReturn();

                    if (refreshTokenJWT.Payload.AccessTokenID != expiredAccessTokenJWT.Payload.ID)
                    {
                        result = OperationResult.Fail("The given refresh token is invalid for the given expired access token").WithoutPayload<SecurityContext>();
                        return;
                    }

                    string userAuthKey = await userAuthInfoStorageResource.GetAuthKeyForUser(expiredAccessTokenJWT.Payload.UserID);

                    OperationResult expiredAccessTokenSignatureValidationResult = await hasher.VerifyMatch(expiredAccessTokenJWT.ToStringUnsigned(), new SecuredHash { Hash = expiredAccessTokenJWT.Signature, Key = userAuthKey });

                    if (!expiredAccessTokenSignatureValidationResult.IsSuccessful)
                    {
                        result = OperationResult.Fail($"The access token signature is invalid which probably means that the token has been tempered with.", expiredAccessTokenSignatureValidationResult.FlattenReasons()).WithoutPayload<SecurityContext>();
                        return;
                    }

                    OperationResult refreshTokenSignatureValidationResult = await hasher.VerifyMatch(refreshTokenJWT.ToStringUnsigned(), new SecuredHash { Hash = refreshTokenJWT.Signature, Key = userAuthKey });

                    if (!refreshTokenSignatureValidationResult.IsSuccessful)
                    {
                        result = OperationResult.Fail($"The refresh token signature is invalid which probably means that the token has been tempered with.", refreshTokenSignatureValidationResult.FlattenReasons()).WithoutPayload<SecurityContext>();
                        return;
                    }

                    AuthInfo brandNewAuthInfo = await AggregateBrandNewAuthInfoForUser(refreshTokenJWT.Payload.UserID);

                    UserInfo userInfo = (await userInfoStorageResource.GetUsersByIds(refreshTokenJWT.Payload.UserID))?.SingleOrDefault();

                    result = OperationResult.Win().WithPayload(new SecurityContext
                    {
                        User = userInfo,
                        Auth = brandNewAuthInfo,
                        //TODO: Populate Roles when implemented
                    });
                })
                .TryOrFailWithGrace(
                    onFail: ex => result = OperationResult.Fail(ex, $"An error occured while validating the provided tokens. See comments for details. Access Token: {expiredAccessToken}; Refresh Token: {refreshToken}").WithoutPayload<SecurityContext>()
                );

            return result;
        }
    }
}
