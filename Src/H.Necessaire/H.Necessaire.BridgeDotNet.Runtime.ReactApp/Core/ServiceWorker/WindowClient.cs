using Bridge;
using static Retyped.es5;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    [External]
    public class WindowClient
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
        [Name("focused")]
        public extern bool IsFocused { get; }

        [External]
        [Name("visibilityState")]
        public extern string VisibilityState { get; }

        [External]
        [Name("frameType")]
        public extern string FrameType { get; }

        [External]
        [Name("focus")]
        public extern Promise<WindowClient> Focus();

        [External]
        [Name("navigate")]
        public extern Promise<WindowClient> Navigate(string url);

        [External]
        [Name("postMessage")]
        public extern void PostMessage(dynamic message, dynamic optionsOrTransferables);
        //https://developer.mozilla.org/en-US/docs/Web/API/Client/postMessage
    }
}
