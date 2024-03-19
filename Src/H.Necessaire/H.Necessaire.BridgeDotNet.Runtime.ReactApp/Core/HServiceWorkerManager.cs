using System;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class HServiceWorkerManager
    {
        public static bool IsWebWorker => Bridge.Html5.Window.Document["isWebWorkerContext"] != null;

        public static void Main()
        {
            if (IsWebWorker)
                return;

            new Action(() =>
            {
                ServiceWorkerContainer
                .Register($"/hsw.js")
                .then<ServiceWorkerRegistration, object>(serviceWorkerRegistration => {

                    ServiceWorkerConsoleLogger.LogInfo("H.Necessaire Service Worker registered");
                    return serviceWorkerRegistration;

                }, error => {
                    ServiceWorkerConsoleLogger.LogError($"Error occurred while trying to register H.Necessaire Service Worker. Error:{Environment.NewLine}{error}");
                    return null;
                });
            })
            .TryOrFailWithGrace(onFail: ex =>
            {
                ServiceWorkerConsoleLogger.LogError($"Error occurred while initializing H.Necessaire Service Worker", ex);
            });
        }
    }
}
