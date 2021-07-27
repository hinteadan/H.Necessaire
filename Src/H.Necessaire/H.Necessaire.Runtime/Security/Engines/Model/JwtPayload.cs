using Newtonsoft.Json;
using System;

namespace H.Necessaire.Runtime.Security.Engines.Model
{
    class JwtPayload : IGuidIdentity
    {
        [JsonProperty("jti")] public Guid ID { get; set; } = Guid.NewGuid();
        [JsonProperty("iss")] public string Issuer { get; set; } = "H.Necessaire";
        [JsonProperty("sub")] public Guid UserID { get; set; }
        [JsonProperty("aud")] public string Audience { get; set; } = "H.Necessaire Consumer";
        [JsonProperty("exp")] public DateTime? ValidUntil { get; set; }
        [JsonProperty("nbf")] public DateTime? ValidFrom { get; set; }
        [JsonProperty("iat")] public DateTime? IssuedAt { get; set; } = DateTime.UtcNow;
    }
}
