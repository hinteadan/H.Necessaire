using H.Necessaire.BridgeDotNet.Runtime.ReactApp.Core.Model.AppState;
using H.Necessaire.BridgeDotNet.Runtime.ReactApp.Resources;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class SecurityManager : ImADependency, ImADependencyGroup
    {
        #region 
        ActionRepeater securityContextUpdateFromLatestLocalStorageRepeater;
        ImALogger logger;
        SecurityContext securityContext;
        HttpClient httpClient;
        SecurityResource securityResource;
        ImAStorageService<string, SecurityContextAppStateEntry> securityContextAppStateResource;
        ImAUserPrivateDataStorage[] userPrivateDataStorages;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            logger = dependencyProvider.GetLogger<SecurityManager>();
            securityResource = dependencyProvider.Get<SecurityResource>();
            httpClient = dependencyProvider.Get<HttpClient>();
            securityContextAppStateResource = dependencyProvider.Get<ImAStorageService<string, SecurityContextAppStateEntry>>();
            userPrivateDataStorages = dependencyProvider.Get<ImAUserPrivateDataStorage[]>();
            securityContextUpdateFromLatestLocalStorageRepeater
                = new ActionRepeater(RefreshSecurityContextWithLatestFromLocalStorage, TimeSpan.FromSeconds(AppBase.Config.Get("SecurityContextCheckIntervalInSeconds")?.ToString()?.ParseToIntOrFallbackTo(30).Value ?? 30));
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

            await securityContextAppStateResource.Save(SecurityContextAppStateEntry.From(securityContext));

            if (securityContext?.Auth != null)
                httpClient.SetAuth(securityContext.Auth.AccessTokenType, securityContext.Auth.AccessToken);

            await securityContextUpdateFromLatestLocalStorageRepeater.Start();

            return loginResult;
        }

        public async Task Logout()
        {
            await securityContextUpdateFromLatestLocalStorageRepeater.Stop();
            await PurgeSecurityContextAndUserPrivateData();
        }

        public async Task RestoreSecurityContextIfPossible()
        {
            await RefreshSecurityContextWithLatestFromLocalStorage();
            await securityContextUpdateFromLatestLocalStorageRepeater.Start();
        }

        private async Task RefreshSecurityContextWithLatestFromLocalStorage()
        {
            SecurityContextAppStateEntry appState
                = (await securityContextAppStateResource.LoadByID(SecurityContextAppStateEntry.DefaultID))?.Payload;

            if (appState == null)
            {
                securityContext = null;
                return;
            }

            if (!appState.IsActive())
            {
                securityContext = null;
                await PurgeSecurityContextAndUserPrivateData();
                return;
            }

            securityContext
                = appState.SecurityContext;

            if (securityContext == null)
            {
                httpClient.ZapAuth();
                return;
            }

            if (securityContext?.Auth != null)
                httpClient.SetAuth(securityContext.Auth.AccessTokenType, securityContext.Auth.AccessToken);
        }

        private async Task PurgeSecurityContextAndUserPrivateData()
        {
            await
                new Func<Task>(async () =>
                {
                    await Task.WhenAll(userPrivateDataStorages.Select(x => x.Purge()).ToArray());
                })
                .TryOrFailWithGrace(
                    onFail: async ex =>
                    {
                        await logger.LogWarn($"Some errors occurred while trying to purge private user data upon logout", ex.Flatten().ToJson());
                    }
                );

            securityContext = null;

            httpClient.ZapAuth();
        }
    }
}
