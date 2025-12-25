using Newtonsoft.Json;
using System;

namespace H.Necessaire.Serialization
{
    public class AbstractConverter<TAbstract, TConcrete>
        : JsonConverter where TConcrete : TAbstract
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(TAbstract);

        public override Object ReadJson(JsonReader reader, Type type, Object value, JsonSerializer jsonSerializer)
            => jsonSerializer.Deserialize<TConcrete>(reader);

        public override void WriteJson(JsonWriter writer, Object value, JsonSerializer jsonSerializer)
            => jsonSerializer.Serialize(writer, value);
    }
}
