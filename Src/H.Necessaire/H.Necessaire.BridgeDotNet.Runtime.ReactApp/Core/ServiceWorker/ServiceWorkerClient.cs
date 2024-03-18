using Bridge;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    [External]
    public class ServiceWorkerClient
    {
        [External]
        [Name("id")]
        public static extern string ID { get; }

        [External]
        [Name("type")]
        public static extern string Type { get; }

        [External]
        [Name("url")]
        public static extern string URL { get; }

        [External]
        [Name("frameType")]
        public static extern string FrameType { get; }

        [External]
        [Name("postMessage")]
        public extern void PostMessage(dynamic message, dynamic optionsOrTransferables);
        //https://developer.mozilla.org/en-US/docs/Web/API/Client/postMessage
    }
}
