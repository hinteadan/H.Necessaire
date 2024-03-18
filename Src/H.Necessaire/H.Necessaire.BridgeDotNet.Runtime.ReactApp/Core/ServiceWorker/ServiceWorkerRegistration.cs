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
        public extern ServiceWorker ActiveServiceWorker { get; }

        [External]
        [Name("installing")]
        public extern ServiceWorker InstallingServiceWorker { get; }

        [External]
        [Name("waiting")]
        public extern ServiceWorker WaitingServiceWorker { get; }

        [External]
        [Name("navigationPreload")]
        public extern NavigationPreloadManager NavigationPreloadManager { get; }

        [External]
        [Name("pushManager")]
        public extern dynamic PushManager { get; }

        [External]
        [Name("scope")]
        public extern string Scope { get; }

        [External]
        [Name("updateViaCache")]
        public extern string UpdateViaCache { get; }

        //https://developer.mozilla.org/en-US/docs/Web/API/ServiceWorkerRegistration/getNotifications
        [External]
        [Name("getNotifications")]
        public extern Promise<dynamic> GetNotifications(dynamic options = null);

        //https://developer.mozilla.org/en-US/docs/Web/API/ServiceWorkerRegistration/showNotification
        [External]
        [Name("showNotification")]
        public extern Promise<object> ShowNotification(string title, dynamic options = null);

        [External]
        [Name("unregister")]
        public extern Promise<bool> Unregister();

        [External]
        [Name("update")]
        public extern Promise<ServiceWorkerRegistration> Update();
    }
}
