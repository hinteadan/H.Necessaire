using Bridge;
using static Retyped.es5;
using static Retyped.dom;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    [External]
    public class ServiceWorkerRegistration : EventTarget
    {
        [External]
        [Name("active")]
        public static extern ServiceWorker ActiveServiceWorker { get; }

        [External]
        [Name("installing")]
        public static extern ServiceWorker InstallingServiceWorker { get; }

        [External]
        [Name("navigationPreload")]
        public static extern NavigationPreloadManager NavigationPreloadManager { get; }
    }
}
