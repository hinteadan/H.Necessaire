using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class SecurityManager : ImADependency, ImADependencyGroup
    {
        #region Construct
        SecurityContext securityContext;
        HttpClient httpClient;
        SecurityResource securityResource;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            securityResource = dependencyProvider.Get<SecurityResource>();
            httpClient = dependencyProvider.Get<HttpClient>();
        }

        public void RegisterDependencies(ImADependencyRegistry dependencyRegistry)
        {
            dependencyRegistry.RegisterAlwaysNew<SecurityContext>(() => securityContext);
        }
        #endregion

        public async Task<OperationResult<SecurityContext>> AuthenticateCredentials(string username, string password)
        {
            OperationResult<SecurityContext> loginResult = await securityResource.Login(new LoginCommand
            {
                Username = username,
                Password = password,
            });

            if (!loginResult.IsSuccessful)
            {
                return loginResult;
            }

            securityContext = loginResult.Payload;

            Bridge.Html5.Window.SessionStorage.SetItem(nameof(SecurityContext), Newtonsoft.Json.JsonConvert.SerializeObject(securityContext));

            if (securityContext?.Auth != null)
                httpClient.SetAuth(securityContext.Auth.AccessTokenType, securityContext.Auth.AccessToken);

            return loginResult;
        }
    }
}
