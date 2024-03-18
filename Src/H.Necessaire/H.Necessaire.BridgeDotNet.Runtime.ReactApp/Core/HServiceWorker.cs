using Bridge;
using Bridge.Html5;
using System;
using static Retyped.es5;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    [Module(ModuleType.UMD, "HServiceWorker")]
    public class HServiceWorker
    {
        public HServiceWorker()
        {

        }

        [Init]
        public static void Main()
        {
            new Action(() =>
            {
                LogInfo("Service worker controller");
                LogInfo(ServiceWorkerContainer.Controller);
            })
            .TryOrFailWithGrace(onFail: ex =>
            {
                LogError("Error occurred while initializing HServiceWorker", ex);
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
