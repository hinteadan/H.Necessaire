using Bridge;
using System;
using static Retyped.dom;
using static Retyped.es5;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    [External]
    public class ExtendableEvent : Event
    {
        public ExtendableEvent(string type) : base(type){}
        public ExtendableEvent(string type, EventInit eventInit) : base(type, eventInit){}

        [External]
        [Name("waitUntil")]
        public extern void WaitUntil(Promise<object> promise);
    }
}
