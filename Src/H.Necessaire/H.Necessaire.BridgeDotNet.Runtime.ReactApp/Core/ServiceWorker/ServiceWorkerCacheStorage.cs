using Bridge;
using static Retyped.dom;
using static Retyped.es5;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    [External]
    public class ServiceWorkerCacheStorage
    {
        [External]
        [Name("delete")]
        public extern Promise<bool> Delete(string cacheName);

        [External]
        [Name("has")]
        public extern Promise<bool> Has(string cacheName);

        [External]
        [Name("keys")]
        public extern Promise<string[]> Keys(string cacheName);

        //https://developer.mozilla.org/en-US/docs/Web/API/CacheStorage/match#options
        [External]
        [Name("match")]
        public extern Promise<object> Match(object request, object options = null);

        [External]
        [Name("open")]
        public extern Promise<ServiceWorkerCache> Open(string cacheName);
    }
}
