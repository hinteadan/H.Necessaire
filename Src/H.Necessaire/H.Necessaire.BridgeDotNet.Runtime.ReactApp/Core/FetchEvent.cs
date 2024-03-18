using Bridge;
using System;
using static Retyped.dom;
using static Retyped.es5;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    [External]
    public class FetchEvent : ExtendableEvent
    {
        public FetchEvent(string typeArg) : base(typeArg) { }
        public FetchEvent(string typeArg, EventInit eventInitDict) : base(typeArg, eventInitDict) { }

        [External]
        [Name("clientId")]
        public static extern string ClientID { get; }

        [External]
        [Name("handled")]
        public static extern Promise<object> Handled { get; }

        [External]
        [Name("preloadResponse")]
        public static extern Promise<dynamic> PreloadResponse { get; }

        [External]
        [Name("replacesClientId")]
        public static extern string ReplacesClientId { get; }

        [External]
        [Name("resultingClientId")]
        public static extern string ResultingClientId { get; }

        [External]
        [Name("request")]
        public static extern dynamic Request { get; }

        [External]
        [Name("respondWith")]
        public extern void RespondWith(dynamic responseOrResponsePromise);
    }
}
