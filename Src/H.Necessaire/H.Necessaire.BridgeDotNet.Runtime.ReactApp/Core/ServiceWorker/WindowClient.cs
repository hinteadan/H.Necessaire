using Bridge;
using static Retyped.es5;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    [External]
    public class WindowClient
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
        [Name("focused")]
        public static extern bool IsFocused { get; }

        [External]
        [Name("visibilityState")]
        public static extern string VisibilityState { get; }

        [External]
        [Name("frameType")]
        public static extern string FrameType { get; }

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
