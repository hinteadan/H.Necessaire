namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public static class Extensions
    {
        public static string ToJsonString<T>(this T obj) => obj.SerializeToJson();
        public static T JsonStringToType<T>(this string jsonString) => jsonString.DeserializeFromJson<T>();
        public static object ToJson<T>(this T obj) => obj.ObjectToJson();
        public static T ToType<T>(this object json) => json.JsonToObject<T>();
        public static T DeepClone<T>(this T data) => data.JsonToObject<T>();
    }
}
