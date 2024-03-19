using Bridge;
using System;
using System.Threading.Tasks;
using static Retyped.dom;
using static Retyped.es5;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    [Module(ModuleType.UMD, nameof(HServiceWorker))]
    public class HServiceWorker
    {
        readonly VersionNumber versionNumber = new VersionNumber(0, 0, 0, null, "play-000003");

        ServiceWorkerGlobalScope serviceWorkerGlobalScope = null;
        Func<object, Promise<object>> fetcher = null;
        HServiceWorker()
        {
            this.serviceWorkerGlobalScope = GetGlobalScopeIfAny();
            this.fetcher = GetFetcher();
        }

        public static void Main() => (new HServiceWorker()).Run();

        public void Run()
        {
            if (serviceWorkerGlobalScope == null)
                return;

            new Action(() =>
            {
                serviceWorkerGlobalScope.addEventListener("install", Install);
                serviceWorkerGlobalScope.addEventListener("fetch", HandleFetch);
            })
            .TryOrFailWithGrace(onFail: ex =>
            {
                ServiceWorkerConsoleLogger.LogError($"Error occurred while starting {nameof(HServiceWorker)}", ex);
            });
        }

        private async void HandleFetch(Event @event)
        {
            FetchEvent fetchEvent = @event.As<FetchEvent>();
            ServiceWorkerConsoleLogger.LogInfo("Fetch event");
            ServiceWorkerConsoleLogger.LogInfo(fetchEvent);

            string cacheID = "v1";
            ServiceWorkerCache cacher = await serviceWorkerGlobalScope.CacheStore.Open(cacheID).ToAsync();
            ServiceWorkerConsoleLogger.LogInfo(cacher);

            object cachedResponse = await (serviceWorkerGlobalScope.CacheStore.Match(fetchEvent.Request).ToAsync());
            if (cachedResponse != null)
            {
                ServiceWorkerConsoleLogger.LogInfo("Responding from cache");
                ServiceWorkerConsoleLogger.LogInfo(cachedResponse);
                fetchEvent.RespondWith(cachedResponse);
                return;
            }

            ServiceWorkerConsoleLogger.LogInfo("Responding from network");
            object response = await fetcher(fetchEvent.Request).ToAsync();
            ServiceWorkerConsoleLogger.LogInfo(cachedResponse);

            fetchEvent.RespondWith(response);
        }

        private async void Install(Event @event)
        {
            await AddResourcesToCache();
        }

        private void Activate()
        {

        }

        private async Task AddResourcesToCache()
        {
            ServiceWorkerConsoleLogger.LogInfo("AddResourcesToCache");
            //string cacheID = "v1";
            //ServiceWorkerCache cacher = await serviceWorkerGlobalScope.CacheStore.Open(cacheID).ToAsync();
            //ServiceWorkerConsoleLogger.LogInfo(cacher);
            //await cacher.AddAll(new string[] {
            //    "/bridge.js",
            //    "/bridge.meta.js",
            //    "/newtonsoft.json.js",
            //    "/ProductiveRage.Immutable.js",
            //    "/ProductiveRage.Immutable.meta.js",
            //    "/H.Necessaire.BridgeDotNet.js",
            //    "/H.Necessaire.BridgeDotNet.meta.js",
            //    "/Bridge.React.js",
            //    "/Bridge.React.meta.js",
            //    "/jquery-2.2.4.js",
            //    "/productiveRage.immutable.extensions.js",
            //    "/productiveRage.immutable.extensions.meta.js",
            //    "/ProductiveRage.ReactRouting.js",
            //    "/ProductiveRage.ReactRouting.meta.js",
            //    "/H.Necessaire.BridgeDotNet.Runtime.ReactApp.js",
            //}).ToAsync();
        }


        private static ServiceWorkerGlobalScope GetGlobalScopeIfAny()
        {
            ServiceWorkerGlobalScope result = null;

            new Action(() =>
            {
                result = Bridge.Script.Get<ServiceWorkerGlobalScope>("$$serviceWorkerGlobalScope");
            })
            .TryOrFailWithGrace(
                onFail: ex => result = null
            );

            return result;
        }

        private static Func<object, Promise<object>> GetFetcher()
        {
            Func<object, Promise<object>> result = null;

            new Action(() =>
            {
                result = Bridge.Script.Get<Func<object, Promise<object>>>("$$fetcher");
            })
            .TryOrFailWithGrace(
                onFail: ex => result = null
            );

            return result;
        }
    }
}
