using Bridge;
using static Retyped.es5;
using static Retyped.dom;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    //https://developer.mozilla.org/en-US/docs/Web/API/WorkerGlobalScope/error_event
    [External]
    public class WorkerGlobalScope : EventTarget
    {
        [External]
        [Name("fonts")]
        public static extern FontFaceSet Fonts { get; }

        [External]
        [Name("location")]
        public static extern WorkerLocation Location { get; }

        [External]
        [Name("navigator")]
        public static extern Navigator Navigator { get; }

        [External]
        [Name("self")]
        public static extern WorkerGlobalScope Self { get; }

        [External]
        [Name("importScripts")]
        public static extern void ImportScripts(params string[] paths);
    }
}
