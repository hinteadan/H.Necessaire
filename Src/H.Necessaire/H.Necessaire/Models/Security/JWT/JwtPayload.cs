using System;
using System.Runtime.Serialization;

namespace H.Necessaire
{
    [DataContract]
    public class JwtPayload : IGuidIdentity
    {
        [DataMember(Name = "jti")] public Guid ID { get; set; } = Guid.NewGuid();

        [DataMember(Name = "iss")] public string Issuer { get; set; } = "H.Necessaire";

        [DataMember(Name = "sub")] public Guid UserID { get; set; }

        [DataMember(Name = "aud")] public string Audience { get; set; } = "H.Necessaire Consumer";

        [DataMember(Name = "exp")] public DateTime? ValidUntil { get; set; }

        [DataMember(Name = "nbf")] public DateTime? ValidFrom { get; set; }

        [DataMember(Name = "iat")] public DateTime? IssuedAt { get; set; } = DateTime.UtcNow;
    }
}
