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
        HServiceWorker()
        {
            this.serviceWorkerGlobalScope = GetGlobalScopeIfAny();
        }

        public static void Main() => (new HServiceWorker()).Run();

        public void Run()
        {
            if (serviceWorkerGlobalScope == null)
                return;

            new Action(() =>
            {
                serviceWorkerGlobalScope.addEventListener("install", Install);
            })
            .TryOrFailWithGrace(onFail: ex =>
            {
                ServiceWorkerConsoleLogger.LogError($"Error occurred while starting {nameof(HServiceWorker)}", ex);
            });
        }


        private void Install(Event @event)
        {
            ExtendableEvent installEvent = @event.As<ExtendableEvent>();

            ServiceWorkerConsoleLogger.LogInfo("Triggered install event");
            ServiceWorkerConsoleLogger.LogInfo(installEvent);

            ServiceWorkerConsoleLogger.LogInfo("Cache Store");
            ServiceWorkerConsoleLogger.LogInfo(serviceWorkerGlobalScope.CacheStore);
        }

        private void Activate()
        {

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
    }
}
