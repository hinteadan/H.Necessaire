using System;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public static class JsonExtensions
    {
        public static string SerializeToJson<T>(this T obj)
        {
            if (obj == null)
                return null;

            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        public static T DeserializeFromJson<T>(this string serializedJson, T defaultTo = default(T))
        {
            if (string.IsNullOrWhiteSpace(serializedJson))
                return defaultTo;

            T result = defaultTo;

            new Action(() =>
            {
                result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(
                    serializedJson,
                    new Newtonsoft.Json.JsonSerializerSettings
                    {
                        ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Replace,
                    }
                );
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
            return Newtonsoft.Json.JsonConvert.DeserializeObject<object>(
                Newtonsoft.Json.JsonConvert.SerializeObject(obj),
                new Newtonsoft.Json.JsonSerializerSettings
                {
                    ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Replace,
                }
            );
        }

        public static T JsonToObject<T>(this object json)
        {
            if (json == null)
                return default(T);
            return
                Newtonsoft.Json.JsonConvert.DeserializeObject<T>(
                    Newtonsoft.Json.JsonConvert.SerializeObject(json),
                    new Newtonsoft.Json.JsonSerializerSettings
                    {
                        ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Replace,
                    }
                );
        }
    }
}
