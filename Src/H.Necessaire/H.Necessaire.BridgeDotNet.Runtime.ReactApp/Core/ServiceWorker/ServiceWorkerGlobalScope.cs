using Bridge;
using static Retyped.dom;
using static Retyped.es5;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    [External]
    public class ServiceWorkerGlobalScope : WorkerGlobalScope
    {
        [External]
        [Name("clients")]
        public static extern ServiceWorkerClients Clients { get; }

        [External]
        [Name("registration")]
        public static extern ServiceWorkerRegistration Registration { get; }

        [External]
        [Name("serviceWorker")]
        public static extern ServiceWorker ServiceWorker { get; }

        [External]
        [Name("skipWaiting")]
        public extern Promise<object> SkipWaiting();
    }
}
