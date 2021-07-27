using Newtonsoft.Json;

namespace H.Necessaire.Runtime.Security.Engines.Model
{
    class JwtHeader
    {
        public static readonly JwtHeader Default = new JwtHeader();

        [JsonProperty("alg")] public string Algorithm { get; set; } = "PBKDF2";
        [JsonProperty("typ")] public string Type { get; set; } = "JWT";
    }
}
