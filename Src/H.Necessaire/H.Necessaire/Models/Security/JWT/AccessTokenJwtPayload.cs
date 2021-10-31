namespace H.Necessaire
{
    public class AccessTokenJwtPayload : JwtPayload
    {
        public UserInfo UserInfo { get; set; }
        public Role[] Roles { get; set; }
    }
}
