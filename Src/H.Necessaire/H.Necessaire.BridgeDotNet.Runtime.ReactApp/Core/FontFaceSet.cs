using Bridge;
using static Retyped.es5;
using static Retyped.dom;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    //https://developer.mozilla.org/en-US/docs/Web/API/FontFaceSet
    [External]
    [Name("FontFaceSet")]
    public class FontFaceSet : EventTarget
    {
        [External]
        [Name("ready")]
        public static extern Promise<FontFaceSet> Ready { get; }

        [External]
        [Name("size")]
        public static extern int Size { get; }

        [External]
        [Name("status")]
        public static extern string Status { get; }

        [External]
        [Name("add")]
        public extern FontFaceSet Add(object fontFace);

    }
}