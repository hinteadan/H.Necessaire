using Bridge;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    [External]
    public class ServiceWorkerClient
    {
        [External]
        [Name("id")]
        public extern string ID { get; }

        [External]
        [Name("type")]
        public extern string Type { get; }

        [External]
        [Name("url")]
        public extern string URL { get; }

        [External]
        [Name("frameType")]
        public extern string FrameType { get; }

        [External]
        [Name("postMessage")]
        public extern void PostMessage(dynamic message, dynamic optionsOrTransferables);
        //https://developer.mozilla.org/en-US/docs/Web/API/Client/postMessage
    }
}
