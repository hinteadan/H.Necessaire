using Newtonsoft.Json;
using System;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public static class JsonExtensions
    {
        static readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            ObjectCreationHandling = ObjectCreationHandling.Replace,
        };

        public static string SerializeToJson<T>(this T obj)
        {
            if (obj == null)
                return null;

            return JsonConvert.SerializeObject(obj, jsonSerializerSettings);
        }

        public static T DeserializeFromJson<T>(this string serializedJson, T defaultTo = default(T))
        {
            if (string.IsNullOrWhiteSpace(serializedJson))
                return defaultTo;

            T result = defaultTo;

            new Action(() =>
            {
                result = JsonConvert.DeserializeObject<T>(serializedJson, jsonSerializerSettings);
            })
            .TryOrFailWithGrace(
                onFail: ex => result = defaultTo
            );

            return result;
        }

        public static object ObjectToJson<T>(this T obj)
        {
            if (obj == null)
                return null;
            return JsonConvert.DeserializeObject<object>(JsonConvert.SerializeObject(obj, jsonSerializerSettings), jsonSerializerSettings);
        }

        public static T JsonToObject<T>(this object json)
        {
            if (json == null)
                return default(T);
            return
                JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(json, jsonSerializerSettings), jsonSerializerSettings);
        }
    }
}
