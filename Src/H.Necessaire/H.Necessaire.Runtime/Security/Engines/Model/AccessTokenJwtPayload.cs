namespace H.Necessaire.Runtime.Security.Engines.Model
{
    class AccessTokenJwtPayload : JwtPayload
    {
        public UserInfo UserInfo { get; set; }
        public Role[] Roles { get; set; }
    }
}
