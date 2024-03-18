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

                    LogInfo("Service Worker registered");
                    LogInfo(serviceWorkerRegistration);
                    return serviceWorkerRegistration;

                }, error => {
                    LogError($"Error occurred while trying to register {nameof(HServiceWorker)}. Error:{Environment.NewLine}{error}");
                    return null;
                });
            })
            .TryOrFailWithGrace(onFail: ex =>
            {
                LogError($"Error occurred while initializing {nameof(HServiceWorker)}", ex);
            });
        }

        private static void LogInfo(object message)
        {
            Bridge.Script.Call("console.info", message);
        }

        private static void LogWarning(string message, Exception ex = null)
        {
            if (ex == null)
            {
                Bridge.Script.Call("console.warn", message);
                return;
            }

            Bridge.Script.Call("console.warn", $"{message}. Message: {ex.Message}.{Environment.NewLine}{Environment.NewLine}{ex}");
        }

        private static void LogError(string message, Exception ex = null)
        {
            if (ex == null)
            {
                Bridge.Script.Call("console.error", message);
                return;
            }

            Bridge.Script.Call("console.error", $"{message}. Message: {ex.Message}.{Environment.NewLine}{Environment.NewLine}{ex}");
        }
    }

    [External]
    [Name("window.navigator.serviceWorker")]
    public class ServiceWorkerContainer
    {
        [External]
        [Name("register")]
        public static extern Promise<ServiceWorkerRegistration> Register(string scriptURL);

        //https://developer.mozilla.org/en-US/docs/Web/API/ServiceWorkerContainer/register
        [External]
        [Name("register")]
        public static extern Promise<ServiceWorkerRegistration> Register(string scriptURL, dynamic options);


        [External]
        [Name("getRegistration")]
        public static extern Promise<ServiceWorkerRegistration> GetRegistration();

        [External]
        [Name("getRegistration")]
        public static extern Promise<ServiceWorkerRegistration> GetRegistration(string clientURL);

        [External]
        [Name("getRegistrations")]
        public static extern Promise<ServiceWorkerRegistration[]> GetRegistrations();

        [External]
        [Name("controller")]
        public static extern ServiceWorker Controller { get; }

        [External]
        [Name("ready")]
        public static extern Promise<ServiceWorkerRegistration> Ready { get; }
    }

    [External]
    public class ServiceWorker
    {

    }

    [External]
    public class ServiceWorkerRegistration
    {

    }
}
