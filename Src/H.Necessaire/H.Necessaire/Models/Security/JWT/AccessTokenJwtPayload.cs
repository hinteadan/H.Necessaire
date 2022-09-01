using System.Runtime.Serialization;

namespace H.Necessaire
{
    [DataContract]
    public class AccessTokenJwtPayload : JwtPayload
    {
        [DataMember] public UserInfo UserInfo { get; set; }
        [DataMember] public Role[] Roles { get; set; }
    }
}
