using Bridge;
using System;
using static Retyped.es5;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    [Module(ModuleType.UMD, nameof(HServiceWorkerManager))]
    public class HServiceWorkerManager
    {
        public HServiceWorkerManager()
        {

        }

        [Init]
        public static void Main()
        {
            new Action(() =>
            {
                ServiceWorkerContainer
                .Register($"/hsw.js")
                .then<ServiceWorkerRegistration, object>(serviceWorkerRegistration => {

                    ServiceWorkerConsoleLogger.LogInfo("Service Worker registered");
                    ServiceWorkerConsoleLogger.LogInfo(serviceWorkerRegistration);
                    return serviceWorkerRegistration;

                }, error => {
                    ServiceWorkerConsoleLogger.LogError($"Error occurred while trying to register {nameof(HServiceWorker)}. Error:{Environment.NewLine}{error}");
                    return null;
                });
            })
            .TryOrFailWithGrace(onFail: ex =>
            {
                ServiceWorkerConsoleLogger.LogError($"Error occurred while initializing {nameof(HServiceWorker)}", ex);
            });
        }
    }
}
