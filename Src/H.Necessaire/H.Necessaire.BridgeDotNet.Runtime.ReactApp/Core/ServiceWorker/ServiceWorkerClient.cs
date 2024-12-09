﻿using Bridge;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    [External]
    [Name("Client")]
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
        public extern void PostMessage(object message, object optionsOrTransferables);
        //https://developer.mozilla.org/en-US/docs/Web/API/Client/postMessage
    }
}