using Google.Cloud.Firestore;
using H.Necessaire.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Core
{
    public static class HsFirestoreExtensions
    {
        const int maxIndexableStringLength = 150;
        const int minIndexableKeywordLength = 3;
        static readonly string[] whitespaces = new string[] { " ", "\t", "\n", "\r" };
        static readonly string[] propertiesExcludedFromTextIndexing = new string[] { "ID", "IDTag", "Value", "DataBinID", "DataPart", "Payload", "PayloadIdentifier", "PayloadType", "PayloadAsJson", "AuditedObjectID", "AuditedObjectType" };

        public static Model.HsFirestoreDocument ToFirestoreDocument<T>(this T data, string id = null, string dataType = null, string dataTypeShortName = null)
        {
            return
                new Model.HsFirestoreDocument
                {
                    Data = data?.ToFirestoreData(),
                    DataType = !dataType.IsEmpty() ? dataType : typeof(T).TypeName(),
                    DataTypeShortName = !dataTypeShortName.IsEmpty() ? dataTypeShortName : typeof(T).Name,
                }
                .And(x =>
                {
                    if (!id.IsEmpty())
                    {
                        x.ID = id;
                    }
                    else if (data != null)
                    {
                        x.ID =
                            (data as IDentityType<Guid>)?.ID.ToString()
                            ?? (data as IDentityType<string>)?.ID.NullIfEmpty()
                            ?? x.ID
                        ;
                    }
                })
                ;
        }

        public static T ToData<T>(this Model.HsFirestoreDocument firestoreDocument)
        {
            if (firestoreDocument?.Data == null)
                return default;

            string json = JsonConvert.SerializeObject(firestoreDocument.Data, GoogleTimeStampConverter.Instance);
            if (json.IsEmpty())
                return default;

            return
                json.JsonToObject<T>();
        }

        public static object ToFirestoreData(this object data)
        {
            if (data == null)
                return null;

            return
                (JsonConvert.DeserializeObject(
                    JsonConvert.SerializeObject(data)
                ) as JToken)
                ?.ToFirestoreData()
                ;
        }

        public static object ToFirestoreData(this JToken jToken)
        {
            if (jToken == null)
                return null;

            switch (jToken.Type)
            {
                case JTokenType.Null:
                case JTokenType.Undefined:
                    return null;

                case JTokenType.Integer:
                    return jToken.Value<long?>();
                case JTokenType.Float:
                    return jToken.Value<double?>();
                case JTokenType.String:
                    return jToken.Value<string>();
                case JTokenType.Boolean:
                    return jToken.Value<bool?>();
                case JTokenType.Date:
                    return jToken.Value<DateTime?>()?.EnsureUtc();

                case JTokenType.Guid:
                    return jToken.Value<Guid?>()?.ToString();
                case JTokenType.Uri:
                    return jToken.Value<Uri>()?.ToString();
                case JTokenType.TimeSpan:
                    return jToken.Value<TimeSpan?>()?.ToString();

                case JTokenType.Bytes:
                    return jToken.Value<byte[]>();

                case JTokenType.Raw:
                    return jToken.Value<object>();


                case JTokenType.Object:
                    return (jToken as JObject).ToFirestoreData();
                case JTokenType.Array:
                    return (jToken as JArray).ToFirestoreData();

                case JTokenType.Constructor:
                case JTokenType.Property:
                case JTokenType.Comment:
                case JTokenType.None:
                default:
                    return null;
            }
        }

        public static object[] ToFirestoreData(this JArray jArray)
        {
            if (jArray?.Any() != true)
                return null;

            return
                jArray
                .Select(x => x.ToFirestoreData())
                .ToNoNullsArray()
                ;
        }

        public static IDictionary<string, object> ToFirestoreData(this JObject jObject)
        {
            if (jObject == null)
                return null;

            IDictionary<string, object> result = new Dictionary<string, object>();

            IEnumerable<JProperty> properties = jObject.Properties();

            if (properties?.Any() != true)
            {
                return result;
            }

            foreach (JProperty property in properties)
            {
                result.Add(property.Name, property.Value.ToFirestoreData());
                AddTextIndexingForPropertyIfNecessary(result, property);
            }

            return result;
        }

        private static void AddTextIndexingForPropertyIfNecessary(IDictionary<string, object> result, JProperty property)
        {
            if (property.Value?.Type != JTokenType.String)
                return;

            if (property.Name.In(propertiesExcludedFromTextIndexing))
                return;

            string value = property.Value?.Value<string>();
            if (value.IsEmpty())
                return;

            value = value.Trim().ToLowerInvariant().Substring(0, Math.Min(maxIndexableStringLength, value.Length));

            result.Add($"{property.Name}AsLowerCase", value);

            result.Add(

                $"{property.Name}AsLowerCaseKeys",

                value
                .Split(whitespaces, StringSplitOptions.RemoveEmptyEntries)
                .Select(x =>
                {
                    if (x.IsEmpty())
                        return null;
                    string k = x.Trim();
                    if (k.Length < minIndexableKeywordLength)
                        return null;
                    return k;
                })
                .ToNoNullsArray()
            );
        }

        class GoogleTimeStampConverter : JsonConverter
        {
            public static readonly GoogleTimeStampConverter Instance = new GoogleTimeStampConverter();

            public override bool CanConvert(Type objectType)
            {
                return objectType.In(typeof(Timestamp));
            }

            public override bool CanRead => false;

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                throw new NotSupportedException();
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteValue(((Timestamp)value).ToDateTime());
            }
        }
    }
}
