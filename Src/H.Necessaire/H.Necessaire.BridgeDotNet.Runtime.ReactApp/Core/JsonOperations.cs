namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public static class JsonExtensions
    {
        public static object ObjectToJson<T>(this T obj)
        {
            if (obj == null)
                return null;
            return Newtonsoft.Json.JsonConvert.DeserializeObject<object>(Newtonsoft.Json.JsonConvert.SerializeObject(obj));
        }

        public static T JsonToObject<T>(this object json)
        {
            if (json == null)
                return default(T);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Newtonsoft.Json.JsonConvert.SerializeObject(json));
        }
    }
}
