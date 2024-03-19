﻿using Bridge;
using System;
using static Retyped.dom;
using static Retyped.es5;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    [External]
    [Name("FetchEvent")]
    public class FetchEvent : ExtendableEvent
    {
        public FetchEvent(string typeArg) : base(typeArg) { }
        public FetchEvent(string typeArg, EventInit eventInitDict) : base(typeArg, eventInitDict) { }

        [External]
        [Name("clientId")]
        public extern string ClientID { get; }

        [External]
        [Name("handled")]
        public extern Promise<object> Handled { get; }

        [External]
        [Name("preloadResponse")]
        public extern Promise<object> PreloadResponse { get; }

        [External]
        [Name("replacesClientId")]
        public extern string ReplacesClientId { get; }

        [External]
        [Name("resultingClientId")]
        public extern string ResultingClientId { get; }

        [External]
        [Name("request")]
        public extern object Request { get; }

        [External]
        [Name("respondWith")]
        public extern void RespondWith(object responseOrResponsePromise);
    }
}
