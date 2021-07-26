using System;

namespace H.Necessaire
{
    public class AuthInfo
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string AccessTokenType { get; set; } = WellKnownAccessTokenType.Bearer;
        public DateTime AccessTokenGeneratedAt { get; set; } = DateTime.UtcNow;
        public TimeSpan AccessTokenValidFor { get; set; }
        public DateTime AccessTokenExpiresAt => AccessTokenGeneratedAt + AccessTokenValidFor;
    }
}
