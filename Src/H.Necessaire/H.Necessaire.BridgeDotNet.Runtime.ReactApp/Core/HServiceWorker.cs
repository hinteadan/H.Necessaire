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
            this.serviceWorkerGlobalScope = GetGlobalScopeIfAny();
        }

        public static async void Main() => await (new HServiceWorker()).Run();

        public async Task Run()
        {
            if (serviceWorkerGlobalScope == null)
            {
                ServiceWorkerConsoleLogger.LogInfo("serviceWorkerGlobalScope IS NULL");
                return;
            }

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
