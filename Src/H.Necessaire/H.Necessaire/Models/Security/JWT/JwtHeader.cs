using System.Runtime.Serialization;

namespace H.Necessaire
{
    [DataContract]
    public class JwtHeader
    {
        public static readonly JwtHeader Default = new JwtHeader();

        [DataMember(Name = "alg")] public string Algorithm { get; set; } = "PBKDF2";

        [DataMember(Name = "typ")] public string Type { get; set; } = "JWT";
    }
}
