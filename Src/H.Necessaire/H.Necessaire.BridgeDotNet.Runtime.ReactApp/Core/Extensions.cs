namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public static class Extensions
    {
        public static object ToJson<T>(this T obj) => obj.ObjectToJson();
        public static T ToType<T>(this object json) => json.JsonToObject<T>();
    }
}
