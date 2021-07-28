using H.Necessaire.Runtime.Security.Managers;
using Microsoft.AspNetCore.Http;
using System;
using System.Text;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Integration.NetCore.Concrete
{
    public class HttpContextToUseCaseContextProvider : ImAUseCaseContextProvider, ImADependency
    {
        #region Construct
        static readonly Encoding basicAuthEncoding = Encoding.GetEncoding("iso-8859-1");
        ImASecurityManager securityManager;
        readonly HttpContext httpContext;
        public HttpContextToUseCaseContextProvider(HttpContext httpContext)
        {
            this.httpContext = httpContext;
        }

        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            securityManager = dependencyProvider.Get<ImASecurityManager>();
        }
        #endregion

        public async Task<UseCaseContext> GetCurrentContext()
        {
            return
                new UseCaseContext
                {
                    SecurityContext = await BuildSecurityContext(),
                };
        }

        private async Task<SecurityContext> BuildSecurityContext()
        {
            if (!httpContext?.Request?.Headers?.ContainsKey("Authorization") ?? true)
                return null;

            string authHeader = httpContext.Request.Headers["Authorization"];

            if (authHeader.StartsWith(WellKnownAccessTokenType.Basic, StringComparison.InvariantCultureIgnoreCase))
                return await BuildSecurityContextFromBasicAuth(authHeader);

            if (authHeader.StartsWith(WellKnownAccessTokenType.Bearer, StringComparison.InvariantCultureIgnoreCase))
                return await BuildSecurityContextFromBearerToken(authHeader);

            if (authHeader.StartsWith(WellKnownAccessTokenType.ApiKey, StringComparison.InvariantCultureIgnoreCase))
                return await BuildSecurityContextFromApiKey(authHeader);

            return null;
        }

        private Task<SecurityContext> BuildSecurityContextFromApiKey(string authHeader)
        {
            throw new NotImplementedException();
        }

        private async Task<SecurityContext> BuildSecurityContextFromBearerToken(string authHeader)
        {
            string authToken = ExtractAccessTokenPartFromAuthHeader(WellKnownAccessTokenType.Bearer, authHeader);

            SecurityContext securityContext = (await securityManager.AuthenticateAccessToken(authToken)).ThrowOnFailOrReturn();

            return securityContext;
        }

        private async Task<SecurityContext> BuildSecurityContextFromBasicAuth(string authHeader)
        {
            string authToken = ExtractAccessTokenPartFromAuthHeader(WellKnownAccessTokenType.Basic, authHeader);

            Note credentials = ParseCredentialsFromBasicAuthHeader(authToken).ThrowOnFailOrReturn();

            SecurityContext securityContext = (await securityManager.AuthenticateCredentials(credentials.Id, credentials.Value)).ThrowOnFailOrReturn();

            return
                securityContext
                .And(x =>
                {
                    x.Auth = new AuthInfo
                    {
                        AccessTokenType = WellKnownAccessTokenType.Basic,
                        AccessToken = authToken,
                    };
                });
        }

        private static string ExtractAccessTokenPartFromAuthHeader(string authScheme, string headerValue)
        {
            return headerValue?.Substring((authScheme?.Length ?? -1) + 1)?.Trim();
        }

        private static OperationResult<Note> ParseCredentialsFromBasicAuthHeader(string basicAuthHeaderValue)
        {
            OperationResult<Note> result = null;

            new Action(() =>
            {
                string usernamePassword = basicAuthEncoding.GetString(Convert.FromBase64String(basicAuthHeaderValue));
                int seperatorIndex = usernamePassword.IndexOf(':');
                string username = usernamePassword.Substring(0, seperatorIndex);
                string password = usernamePassword.Substring(seperatorIndex + 1);
                result = OperationResult.Win().WithPayload(new Note(username, password));
            })
            .TryOrFailWithGrace(
                onFail: x => result = OperationResult.Fail(x, $"The Basic Authentication Header is invalid. Value: {basicAuthHeaderValue}").WithoutPayload<Note>()
            );

            return result;
        }
    }
}
