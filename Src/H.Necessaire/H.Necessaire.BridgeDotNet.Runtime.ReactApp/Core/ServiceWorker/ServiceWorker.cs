using Bridge;
using static Retyped.dom;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    [External]
    [Name("ServiceWorker")]
    public class ServiceWorker : EventTarget
    {
        [External]
        [Name("scriptURL")]
        public extern string ScriptURL { get; }

        [External]
        [Name("state")]
        public extern string State { get; }

        [External]
        [Name("postMessage")]
        public extern void PostMessage(object message, object optionsOrTransferables);
        //https://developer.mozilla.org/en-US/docs/Web/API/ServiceWorker/postMessage
    }
}
