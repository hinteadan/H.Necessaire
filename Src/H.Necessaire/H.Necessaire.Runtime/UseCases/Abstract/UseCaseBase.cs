using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime
{
    public abstract class UseCaseBase : ImAUseCase, ImADependency
    {
        #region Construct
        const int httpStatusCodeUnauthorized = 401;

        const string noAuthReason = "This use case requires authentication. The current request is anonymous. Please authenticate and try again.";
        private ImAUseCaseContextProvider contextProvider;
        private ImALogger logger;
        public virtual void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            CallContext<Guid?>.SetData(CallContextKey.LoggingScopeID, Guid.NewGuid());
            CallContext<OperationContext>.SetData(CallContextKey.OperationContext, GetCurrentContext().ContinueWith(x => x.Result?.OperationContext).GetAwaiter().GetResult());

            contextProvider = dependencyProvider.Get<ImAUseCaseContextProvider>();
            logger = dependencyProvider.GetLogger(this.GetType(), "H.Necessaire.Runtime");
        }
        #endregion

        protected ImALogger Logger => logger;

        protected async Task<UseCaseContext> GetCurrentContext()
        {
            if (contextProvider == null)
                return null;

            return
                await contextProvider.GetCurrentContext();
        }

        protected async Task<OperationResult<UseCaseContext>> EnsureAuthentication()
        {
            UseCaseContext useCaseContext = await GetCurrentContext();

            OperationResult<UseCaseContext> result =
                useCaseContext?.SecurityContext == null
                ? OperationResult
                .Fail(noAuthReason)
                .WithPayload(
                    (useCaseContext ?? new UseCaseContext())
                    .And(x => x.FailContext = new UseCaseFailContext
                    {
                        ReasonCode = httpStatusCodeUnauthorized,
                        ReasonPhrase = noAuthReason,
                    })
                )
                : OperationResult.Win().WithPayload(useCaseContext);

            return result;
        }

        protected async Task<OperationResult<UseCaseContext>> EnsureAuthenticationType(params string[] acceptedAuthTypes)
        {
            OperationResult<UseCaseContext> result = await EnsureAuthentication();

            if (!acceptedAuthTypes?.Any() ?? true)
            {
                result
                    = OperationResult
                    .Fail("Accepted auth types are NOT specified, therefore any auth type is denied.")
                    .WithPayload(
                        (result?.Payload ?? new UseCaseContext())
                        .And(x => x.FailContext = new UseCaseFailContext
                        {
                            ReasonCode = httpStatusCodeUnauthorized,
                            ReasonPhrase = "Accepted auth types are NOT specified, therefore any auth type is denied.",
                        })
                    );
                return result;
            }

            if (result?.Payload?.SecurityContext?.Auth?.AccessTokenType == null)
            {
                result
                    = OperationResult
                    .Fail("Access token type is NOT specfied in the current context, therefore access is denied")
                    .WithPayload(
                        (result?.Payload ?? new UseCaseContext())
                        .And(x => x.FailContext = new UseCaseFailContext
                        {
                            ReasonCode = httpStatusCodeUnauthorized,
                            ReasonPhrase = "Access token type is NOT specfied in the current context, therefore access is denied",
                        })
                    );
                return result;
            }

            if (!acceptedAuthTypes.Any(x => string.Equals(x, result.Payload.SecurityContext.Auth.AccessTokenType, StringComparison.InvariantCultureIgnoreCase)))
            {
                string reason = $"The current access token type ({result.Payload.SecurityContext.Auth.AccessTokenType}) is not accepted for the current use case, therefore access is denied. You must use one of the following authentication methods: {string.Join(", ", acceptedAuthTypes)}";
                result
                    = OperationResult
                    .Fail(reason)
                    .WithPayload(
                        (result?.Payload ?? new UseCaseContext())
                        .And(x => x.FailContext = new UseCaseFailContext
                        {
                            ReasonCode = httpStatusCodeUnauthorized,
                            ReasonPhrase = reason,
                        })
                    );
                return result;
            }

            return result;
        }

        protected async Task<OperationResult<UseCaseContext>> EnsureAuthenticationAndPermissions(params PermissionClaim[] permissionClaims)
        {
            OperationResult<UseCaseContext> authResult = await EnsureAuthentication();
            if (!authResult.IsSuccessful)
                return authResult;

            return CheckPermissions(authResult.Payload?.SecurityContext, permissionClaims).WithPayload(authResult.Payload);
        }

        protected async Task<OperationResult<UseCaseContext>> EnsureAuthenticationTypeAndPermissions(string[] acceptedAuthTypes, params PermissionClaim[] permissionClaims)
        {
            OperationResult<UseCaseContext> authResult = await EnsureAuthenticationType(acceptedAuthTypes);
            if (!authResult.IsSuccessful)
                return authResult;

            return CheckPermissions(authResult.Payload?.SecurityContext, permissionClaims).WithPayload(authResult.Payload);
        }

        private OperationResult CheckPermissions(SecurityContext securityContext, params PermissionClaim[] permissionClaims)
        {
            bool hasPermission = securityContext?.HasPermission(permissionClaims) == true;
            if (!hasPermission)
                return OperationResult.Fail($"{securityContext?.User?.Username} doesn't have permissions to execute this operation");

            return OperationResult.Win();
        }
    }
}
