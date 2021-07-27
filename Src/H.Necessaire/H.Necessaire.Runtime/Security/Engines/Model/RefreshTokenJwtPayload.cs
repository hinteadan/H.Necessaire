using System;

namespace H.Necessaire.Runtime.Security.Engines.Model
{
    class RefreshTokenJwtPayload : JwtPayload
    {
        public Guid AccessTokenID { get; set; }
    }
}
