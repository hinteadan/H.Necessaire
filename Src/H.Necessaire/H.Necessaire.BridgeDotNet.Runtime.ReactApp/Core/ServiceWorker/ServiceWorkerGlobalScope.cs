using Bridge;
using System;
using static Retyped.es5;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    [External]
    public class ServiceWorkerGlobalScope
    {
        [External]
        [Name("addEventListener")]
        public extern void AddEventListener(string eventID, Action<ExtendableEvent> eventHandler);

        [External]
        [Name("skipWaiting")]
        public extern Promise<object> SkipWaiting();
    }
}
