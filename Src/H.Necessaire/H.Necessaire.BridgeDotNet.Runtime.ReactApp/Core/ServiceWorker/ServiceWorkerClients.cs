using Bridge;
using static Retyped.es5;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    [External]
    public class ServiceWorkerClients
    {
        [External]
        [Name("claim")]
        public extern Promise<object> Claim();

        [External]
        [Name("get")]
        public extern Promise<ServiceWorkerClient> Get(string id);

        [External]
        [Name("matchAll")]
        public extern Promise<ServiceWorkerClient[]> MatchAll();

        [External]
        [Name("matchAll")]
        public extern Promise<ServiceWorkerClient[]> MatchAll(dynamic options);
        //https://developer.mozilla.org/en-US/docs/Web/API/Clients/matchAll#includeuncontrolled

        [External]
        [Name("openWindow")]
        public extern Promise<WindowClient> OpenWindow(string url);
    }
}
