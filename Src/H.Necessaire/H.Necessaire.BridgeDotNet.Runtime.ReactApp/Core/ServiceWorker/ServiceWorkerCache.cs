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
        public extern Promise<object> Add(object request);

        [External]
        [Name("put")]
        public extern Promise<object> Put(object request, object response);

        [External]
        [Name("addAll")]
        public extern Promise<object> AddAll(object[] requests);

        //https://developer.mozilla.org/en-US/docs/Web/API/Cache/delete#options
        [External]
        [Name("delete")]
        public extern Promise<bool> Delete(object request, object options = null);

        //https://developer.mozilla.org/en-US/docs/Web/API/Cache/keys#options
        [External]
        [Name("keys")]
        public extern Promise<object[]> Keys(object request = null, object options = null);

        //https://developer.mozilla.org/en-US/docs/Web/API/Cache/match#options
        [External]
        [Name("match")]
        public extern Promise<object> Match(object request, object options = null);

        //https://developer.mozilla.org/en-US/docs/Web/API/Cache/matchAll#options
        [External]
        [Name("matchAll")]
        public extern Promise<object[]> MatchAll(object request = null, object options = null);
    }
}