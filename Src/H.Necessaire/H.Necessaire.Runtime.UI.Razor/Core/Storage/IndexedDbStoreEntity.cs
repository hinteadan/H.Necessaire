using Newtonsoft.Json;
using System.Text.Json.Serialization;
using Tavenem.DataStorage;

namespace H.Necessaire.Runtime.UI.Razor.Core.Storage
{
    internal class IndexedDbStoreEntity<T, TID> : IIdItem where T : IDentityType<TID>
    {
        [JsonProperty("id")]
        [JsonPropertyName("id"), JsonInclude, JsonPropertyOrder(-2)]
        public string Id => Data?.ID?.ToString();

        [JsonProperty("_id_t")]
        [JsonPropertyName("_id_t"), JsonInclude, JsonPropertyOrder(-1)]
        public string IdItemTypeName { get; init; } = typeof(T).Name;

        public T Data { get; set; }

        public static implicit operator T(IndexedDbStoreEntity<T, TID> x) => x.Data;
        public static implicit operator IndexedDbStoreEntity<T, TID>(T x) => new IndexedDbStoreEntity<T, TID> { Data = x };
    }
}
