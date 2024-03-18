using Bridge;
using static Retyped.dom;
using static Retyped.es5;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    [External]
    public class ServiceWorkerCache
    {
        [External]
        [Name("add")]
        public extern Promise<object> Add(dynamic request);

        [External]
        [Name("put")]
        public extern Promise<object> Put(dynamic request, dynamic response);

        [External]
        [Name("addAll")]
        public extern Promise<object> AddAll(dynamic[] requests);

        //https://developer.mozilla.org/en-US/docs/Web/API/Cache/delete#options
        [External]
        [Name("delete")]
        public extern Promise<bool> Delete(dynamic request, dynamic options = null);

        //https://developer.mozilla.org/en-US/docs/Web/API/Cache/keys#options
        [External]
        [Name("keys")]
        public extern Promise<dynamic[]> Keys(dynamic request = null, dynamic options = null);

        //https://developer.mozilla.org/en-US/docs/Web/API/Cache/match#options
        [External]
        [Name("match")]
        public extern Promise<dynamic> Match(dynamic request, dynamic options = null);

        //https://developer.mozilla.org/en-US/docs/Web/API/Cache/matchAll#options
        [External]
        [Name("matchAll")]
        public extern Promise<dynamic[]> MatchAll(dynamic request = null, dynamic options = null);
    }
}