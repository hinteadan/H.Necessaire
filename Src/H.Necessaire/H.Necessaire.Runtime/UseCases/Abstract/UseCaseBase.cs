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
        private UseCaseContext currentContext = null;
        private ImAUseCaseContextProvider contextProvider;
        public virtual void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            contextProvider = dependencyProvider.Get<ImAUseCaseContextProvider>();
        }
        #endregion

        protected async Task<UseCaseContext> GetCurrentContext()
        {
            if (contextProvider == null)
                return null;

            if (currentContext != null)
                return currentContext;

            currentContext = await contextProvider.GetCurrentContext();

            return currentContext;
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

            return result.And(x => x.ThrowOnFail());
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
                return result.And(x => x.ThrowOnFail());
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
                return result.And(x => x.ThrowOnFail());
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
                return result.And(x => x.ThrowOnFail());
            }

            return result.And(x => x.ThrowOnFail());
        }
    }
}
