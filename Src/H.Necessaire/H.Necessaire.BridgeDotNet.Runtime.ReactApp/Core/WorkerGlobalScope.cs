using Bridge;
using static Retyped.es5;
using static Retyped.dom;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    //https://developer.mozilla.org/en-US/docs/Web/API/WorkerGlobalScope/error_event
    [External]
    [Name("WorkerGlobalScope")]
    public class WorkerGlobalScope : EventTarget
    {
        [External]
        [Name("fonts")]
        public extern FontFaceSet Fonts { get; }

        [External]
        [Name("location")]
        public extern WorkerLocation Location { get; }

        [External]
        [Name("navigator")]
        public extern Navigator Navigator { get; }

        [External]
        [Name("self")]
        public extern WorkerGlobalScope Self { get; }

        [External]
        [Name("importScripts")]
        public extern void ImportScripts(params string[] paths);

        [External]
        [Name("fetch")]
        public extern Promise<Response> Fetch(Request request, object options = null);

        [External]
        [Name("fetch")]
        public extern Promise<Response> Fetch(string url, object options = null);
    }
}
