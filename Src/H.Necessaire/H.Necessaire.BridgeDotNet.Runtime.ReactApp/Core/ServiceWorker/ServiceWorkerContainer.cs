using Bridge;
using System;
using static Retyped.es5;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    [External]
    [Name("window.navigator.serviceWorker")]
    public class ServiceWorkerContainer
    {
        [External]
        [Name("register")]
        public static extern Promise<ServiceWorkerRegistration> Register(string scriptURL);

        //https://developer.mozilla.org/en-US/docs/Web/API/ServiceWorkerContainer/register
        [External]
        [Name("register")]
        public static extern Promise<ServiceWorkerRegistration> Register(string scriptURL, dynamic options);


        [External]
        [Name("getRegistration")]
        public static extern Promise<ServiceWorkerRegistration> GetRegistration();

        [External]
        [Name("getRegistration")]
        public static extern Promise<ServiceWorkerRegistration> GetRegistration(string clientURL);

        [External]
        [Name("getRegistrations")]
        public static extern Promise<ServiceWorkerRegistration[]> GetRegistrations();

        [External]
        [Name("controller")]
        public static extern ServiceWorker Controller { get; }

        [External]
        [Name("ready")]
        public static extern Promise<ServiceWorkerRegistration> Ready { get; }
    }
}
