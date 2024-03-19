using Bridge;
using static Retyped.dom;
using static Retyped.es5;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    [External]
    [Name("ServiceWorkerGlobalScope")]
    public class ServiceWorkerGlobalScope : WorkerGlobalScope
    {
        [External]
        [Name("caches")]
        public extern ServiceWorkerCacheStorage CacheStore { get; }

        [External]
        [Name("clients")]
        public extern ServiceWorkerClients Clients { get; }

        [External]
        [Name("registration")]
        public extern ServiceWorkerRegistration Registration { get; }

        [External]
        [Name("serviceWorker")]
        public extern ServiceWorker ServiceWorker { get; }

        [External]
        [Name("skipWaiting")]
        public extern Promise<object> SkipWaiting();
    }
}
