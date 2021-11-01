using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class SecurityResource : HttpApiResourceBase
    {
        #region Construct
        string loginUrl;
        string refreshUrl;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);

            loginUrl = $"{BaseAiUrl}{AppBase.Config.Get("Security")?.Get("LoginUrl")?.ToString() ?? "/Security/Login"}";
            refreshUrl = $"{BaseAiUrl}{AppBase.Config.Get("Security")?.Get("RefreshUrl")?.ToString() ?? "/Security/Refresh"}";
        }
        #endregion

        public async Task<OperationResult<SecurityContext>> Login(LoginCommand command)
        {
            OperationResult<OperationResult<SecurityContext>> httpRequestResult =
                await SafelyRequest(() => httpClient.PostJson<OperationResult<SecurityContext>>(loginUrl, command));

            if (!httpRequestResult.IsSuccessful)
                return OperationResult.Fail("A request error occured while trying to login", httpRequestResult.FlattenReasons()).WithoutPayload<SecurityContext>();

            return httpRequestResult.Payload;
        }

        public async Task<OperationResult<SecurityContext>> Refresh(RefreshAccessTokenCommand command)
        {
            OperationResult<OperationResult<SecurityContext>> httpRequestResult =
                await SafelyRequest(() => httpClient.PostJson<OperationResult<SecurityContext>>(refreshUrl, command));

            if (!httpRequestResult.IsSuccessful)
                return OperationResult.Fail("A request error occured while trying to refresh the access token", httpRequestResult.FlattenReasons()).WithoutPayload<SecurityContext>();

            return httpRequestResult.Payload;
        }
    }
}
