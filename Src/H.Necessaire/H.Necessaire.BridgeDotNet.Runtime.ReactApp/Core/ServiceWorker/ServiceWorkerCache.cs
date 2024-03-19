using Bridge;
using static Retyped.dom;
using static Retyped.es5;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    [External]
    [Name("Cache")]
    public class ServiceWorkerCache
    {
        [External]
        [Name("add")]
        public extern Promise<object> Add(Request request);
        [External]
        [Name("add")]
        public extern Promise<object> Add(string url);

        [External]
        [Name("put")]
        public extern Promise<object> Put(Request request, Response response);
        [External]
        [Name("put")]
        public extern Promise<object> Put(string url, Response response);

        [External]
        [Name("addAll")]
        public extern Promise<object> AddAll(Request[] requests);
        [External]
        [Name("addAll")]
        public extern Promise<object> AddAll(string[] urls);

        //https://developer.mozilla.org/en-US/docs/Web/API/Cache/delete#options
        [External]
        [Name("delete")]
        public extern Promise<bool> Delete(Request request, object options = null);
        //https://developer.mozilla.org/en-US/docs/Web/API/Cache/delete#options
        [External]
        [Name("delete")]
        public extern Promise<bool> Delete(string url, object options = null);

        //https://developer.mozilla.org/en-US/docs/Web/API/Cache/keys#options
        [External]
        [Name("keys")]
        public extern Promise<Request[]> Keys(Request request = null, object options = null);
        //https://developer.mozilla.org/en-US/docs/Web/API/Cache/keys#options
        [External]
        [Name("keys")]
        public extern Promise<Request[]> Keys(string url = null, object options = null);

        //https://developer.mozilla.org/en-US/docs/Web/API/Cache/match#options
        [External]
        [Name("match")]
        public extern Promise<Response> Match(Request request, object options = null);
        //https://developer.mozilla.org/en-US/docs/Web/API/Cache/match#options
        [External]
        [Name("match")]
        public extern Promise<Response> Match(string url, object options = null);

        //https://developer.mozilla.org/en-US/docs/Web/API/Cache/matchAll#options
        [External]
        [Name("matchAll")]
        public extern Promise<Response[]> MatchAll(Request request = null, object options = null);
        //https://developer.mozilla.org/en-US/docs/Web/API/Cache/matchAll#options
        [External]
        [Name("matchAll")]
        public extern Promise<Response[]> MatchAll(string url = null, object options = null);
    }
}