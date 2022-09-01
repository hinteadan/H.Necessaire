using System;
using System.Runtime.Serialization;

namespace H.Necessaire
{
    [DataContract]
    public class RefreshTokenJwtPayload : JwtPayload
    {
        [DataMember] public Guid AccessTokenID { get; set; }
    }
}
