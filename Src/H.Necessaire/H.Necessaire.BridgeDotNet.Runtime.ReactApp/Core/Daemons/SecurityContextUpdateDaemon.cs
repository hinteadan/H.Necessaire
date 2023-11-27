using H.Necessaire.BridgeDotNet.Runtime.ReactApp.Core.Model.AppState;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class SecurityContextUpdateDaemon : WebWorkerDaemonBase
    {
        public SecurityContextUpdateDaemon(Func<ImAnAppWireup> webWorkerWireup = null, params string[] scriptsToInclude)
            : base(
                () => AppBase.Get<Worker>(),
                webWorkerWireup,
                scriptsToInclude)
        {
        }

        public class Worker : ImAWebWorkerDaemonAction, ImADependency
        {
            #region Construct
            ActionRepeater repeater;
            ImALogger logger;
            SecurityResource securityResource;
            ImAStorageService<string, SecurityContextAppStateEntry> securityContextAppStateResource;
            HttpClient httpClient;
            public void ReferDependencies(ImADependencyProvider dependencyProvider)
            {
                logger = dependencyProvider.GetLogger<Worker>();
                repeater = new ActionRepeater(RunRefreshCycle, TimeSpan.FromSeconds(AppBase.Config.Get("SecurityContextCheckIntervalInSeconds")?.ToString()?.ParseToIntOrFallbackTo(30).Value ?? 30));
                securityResource = dependencyProvider.Get<SecurityResource>();
                securityContextAppStateResource = dependencyProvider.Get<ImAStorageService<string, SecurityContextAppStateEntry>>();
                httpClient = dependencyProvider.Get<HttpClient>();
            }
            #endregion

            public async void DoWork()
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
                await repeater.Start();
            }

            private async Task RunRefreshCycle()
            {
                if (!AppBase.IsOnline)
                {
                    await logger.LogTrace($"Skipping SecurityContext Refresh Cycle because we're offline");
                    return;
                }

                await logger.LogDebug($"Running SecurityContext Refresh Cycle");

                using (new TimeMeasurement(async x => await logger.LogDebug($"DONE Running SecurityContext Refresh Cycle in {x}")))
                {
                    SecurityContextAppStateEntry securityContextAppStateEntry
                        = (await securityContextAppStateResource.LoadByID(SecurityContextAppStateEntry.DefaultID))?.Payload;

                    if (string.IsNullOrWhiteSpace(securityContextAppStateEntry?.Payload?.Auth?.AccessToken))
                    {
                        await logger.LogDebug($"Skipping SecurityContext Refresh Cycle because there's no auth context");
                        return;
                    }

                    if (securityContextAppStateEntry?.IsActive() == true)
                    {
                        await logger.LogDebug($"Skipping SecurityContext Refresh Cycle because the current token is still valid");
                        return;
                    }

                    OperationResult<SecurityContext> tokenRefreshOperation
                        = await securityResource.Refresh(new RefreshAccessTokenCommand
                        {
                            ExpiredAccessToken = securityContextAppStateEntry?.Payload?.Auth?.AccessToken,
                            RefreshToken = securityContextAppStateEntry?.Payload?.Auth?.RefreshToken,
                        });

                    if (!tokenRefreshOperation.IsSuccessful)
                    {
                        await logger.LogError($"Error occurred while trying to refresh the expired auth token", new OperationResultException(tokenRefreshOperation), securityContextAppStateEntry?.Payload?.Auth?.ToJson());
                        return;
                    }

                    await securityContextAppStateResource.Save(SecurityContextAppStateEntry.From(tokenRefreshOperation.Payload));
                }
            }
        }
    }
}
