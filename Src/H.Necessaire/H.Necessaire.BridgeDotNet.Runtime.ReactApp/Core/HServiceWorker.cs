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
        readonly VersionNumber versionNumber = new VersionNumber(0, 0, 0, null, "play-000001");

        ServiceWorkerGlobalScope serviceWorkerGlobalScope = null;
        Func<Request, Promise<Response>> fetcher = null;
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

            string httpMethod = fetchEvent.Request["method"].As<string>();
            if (!httpMethod.Is("GET"))
            {
                Response res = await fetcher(fetchEvent.Request).ToAsync();

                ServiceWorkerConsoleLogger.LogInfo($"Fetch event method is {httpMethod}, not GET, therefore skipping cache...");
                ServiceWorkerConsoleLogger.LogInfo(fetchEvent.Request);
                ServiceWorkerConsoleLogger.LogInfo(res);

                fetchEvent.RespondWith(await fetcher(fetchEvent.Request).ToAsync());
                return;
            }


            ServiceWorkerCache cacher = await OpenCurrentCacher();

            Response cachedResponse = await cacher.Match(fetchEvent.Request).ToAsync();
            if (cachedResponse != null)
            {
                fetchEvent.RespondWith(cachedResponse);
                return;
            }

            Response networkResponse = await fetcher(fetchEvent.Request).ToAsync();

            ServiceWorkerConsoleLogger.LogInfo("Request not cached, responding from network");
            ServiceWorkerConsoleLogger.LogInfo(fetchEvent.Request);
            ServiceWorkerConsoleLogger.LogInfo(networkResponse);

            fetchEvent.RespondWith(networkResponse);
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
            ServiceWorkerCache cacher = await OpenCurrentCacher();

            await cacher.AddAll(new Union<Request, string>[] {
                "/dexie.js",
                "/react.production.min.js",
                "/react-dom.production.min.js",
                "/marked.min.js",
                "/highlight.min.js",
                "/bridge.js",
                "/bridge.meta.js",
                "/newtonsoft.json.js",
                "/ProductiveRage.Immutable.js",
                "/ProductiveRage.Immutable.meta.js",
                "/H.Necessaire.BridgeDotNet.js",
                "/H.Necessaire.BridgeDotNet.meta.js",
                "/Bridge.React.js",
                "/Bridge.React.meta.js",
                "/jquery-2.2.4.js",
                "/productiveRage.immutable.extensions.js",
                "/productiveRage.immutable.extensions.meta.js",
                "/ProductiveRage.ReactRouting.js",
                "/ProductiveRage.ReactRouting.meta.js",
                "/H.Necessaire.BridgeDotNet.Runtime.ReactApp.js",
            }).ToAsync();
        }

        private async Task<ServiceWorkerCache> OpenCurrentCacher()
        {
            string cacheID = $"H.Necessaire-Cache-{versionNumber}";
            return await serviceWorkerGlobalScope.CacheStore.Open(cacheID).ToAsync();
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

        private static Func<Request, Promise<Response>> GetFetcher()
        {
            Func<Request, Promise<Response>> result = null;

            new Action(() =>
            {
                result = Bridge.Script.Get<Func<Request, Promise<Response>>>("$$fetcher");
            })
            .TryOrFailWithGrace(
                onFail: ex => result = null
            );

            return result;
        }
    }
}
