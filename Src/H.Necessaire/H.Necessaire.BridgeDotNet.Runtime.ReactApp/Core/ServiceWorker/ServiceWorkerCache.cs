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
        public extern Promise<object> Add(Union<Request, string> request);

        [External]
        [Name("put")]
        public extern Promise<object> Put(Union<Request, string> request, Response response);

        [External]
        [Name("addAll")]
        public extern Promise<object> AddAll(Union<Request, string>[] requests);

        //https://developer.mozilla.org/en-US/docs/Web/API/Cache/delete#options
        [External]
        [Name("delete")]
        public extern Promise<bool> Delete(Union<Request, string> requestOrUrl, object options = null);

        //https://developer.mozilla.org/en-US/docs/Web/API/Cache/keys#options
        [External]
        [Name("keys")]
        public extern Promise<Request[]> Keys(Union<Request, string> requestOrUrl = null, object options = null);

        //https://developer.mozilla.org/en-US/docs/Web/API/Cache/match#options
        [External]
        [Name("match")]
        public extern Promise<Response> Match(Union<Request, string> request, object options = null);

        //https://developer.mozilla.org/en-US/docs/Web/API/Cache/matchAll#options
        [External]
        [Name("matchAll")]
        public extern Promise<Response[]> MatchAll(Union<Request, string> request = null, object options = null);
    }
}