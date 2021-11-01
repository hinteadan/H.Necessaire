using Bridge;


namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    [External]
    [Name("window.URL")]
    public class JsURL
    {
        [External]
        public static extern string createObjectURL(object payload);

        [External]
        public static extern void revokeObjectURL(string url);
    }
}
