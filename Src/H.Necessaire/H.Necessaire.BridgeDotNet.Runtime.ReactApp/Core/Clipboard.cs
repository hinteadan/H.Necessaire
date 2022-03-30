using Bridge;
using static Retyped.es5;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp.Core
{
    [External]
    [Name("navigator.clipboard")]
    public class Clipboard
    {
        [External] public static extern Promise<object> writeText(string text);

        [External] public static extern Promise<string> readText();
    }
}
