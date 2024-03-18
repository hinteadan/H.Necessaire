using Bridge;
using System;
using System.Threading.Tasks;
using static Retyped.es5;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    [Module(ModuleType.UMD, nameof(HServiceWorker))]
    public class HServiceWorker
    {
        ServiceWorkerGlobalScope serviceWorkerGlobalScope = null;
        HServiceWorker()
        {
            serviceWorkerGlobalScope = GetGlobalScopeIfAny();
        }

        [Init] public static async void Main() => await new HServiceWorker().Run();

        public async Task Run()
        {
            new Action(() =>
            {
                ServiceWorkerConsoleLogger.LogInfo("I'm inside the Service Worker");
                if (serviceWorkerGlobalScope == null)
                {
                    ServiceWorkerConsoleLogger.LogInfo("serviceWorkerGlobalScope IS NULL");
                    return;
                }

                
                ServiceWorkerConsoleLogger.LogInfo(serviceWorkerGlobalScope);

            })
            .TryOrFailWithGrace(onFail: ex =>
            {
                ServiceWorkerConsoleLogger.LogError($"Error occurred while starting {nameof(HServiceWorker)}", ex);
            });
        }


        private static ServiceWorkerGlobalScope GetGlobalScopeIfAny()
        {
            ServiceWorkerGlobalScope serviceWorkerGlobalScope = null;

            new Action(() =>
            {
                serviceWorkerGlobalScope = Bridge.Script.Get<ServiceWorkerGlobalScope>("serviceWorkerGlobalScope");
            })
            .TryOrFailWithGrace(
                onFail: ex => serviceWorkerGlobalScope = null
            );

            return serviceWorkerGlobalScope;
        }
    }
}
