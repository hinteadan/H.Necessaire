using System;

namespace H.Necessaire
{
    public class RefreshTokenJwtPayload : JwtPayload
    {
        public Guid AccessTokenID { get; set; }
    }
}
