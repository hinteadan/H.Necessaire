using Bridge;
using static Retyped.dom;
using static Retyped.es5;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    [External]
    [Name("NavigationPreloadManager")]
    public class NavigationPreloadManager
    {
        [External]
        [Name("disable")]
        public extern Promise<object> Disable();

        [External]
        [Name("enable")]
        public extern Promise<object> Enable();

        //https://developer.mozilla.org/en-US/docs/Web/API/NavigationPreloadManager/getState
        [External]
        [Name("getState")]
        public extern Promise<object> GetState();

        [External]
        [Name("setHeaderValue")]
        public extern Promise<object> SetHeaderValue(string value);
    }
}