using Bridge;
using System;
using static Retyped.es5;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    [Module(ModuleType.UMD, nameof(HServiceWorker), preventModuleName: true)]
    public class HServiceWorker
    {
        readonly ServiceWorkerGlobalScope serviceWorkerGlobalScope;
        HServiceWorker()
        {
            serviceWorkerGlobalScope = Bridge.Script.Eval<ServiceWorkerGlobalScope>("serviceWorkerGlobalScope");
        }

        public void Run()
        {
            new Action(() =>
            {
                if (serviceWorkerGlobalScope == null)
                    return;

                ServiceWorkerConsoleLogger.LogInfo("I'm inside the Service Worker");
                ServiceWorkerConsoleLogger.LogInfo(serviceWorkerGlobalScope);

            })
            .TryOrFailWithGrace(onFail: ex =>
            {
                ServiceWorkerConsoleLogger.LogError($"Error occurred while starting {nameof(HServiceWorker)}", ex);
            });
        }
    }
}
