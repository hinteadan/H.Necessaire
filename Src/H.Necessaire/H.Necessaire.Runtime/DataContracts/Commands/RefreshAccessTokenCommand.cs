namespace H.Necessaire.Runtime
{
    public class RefreshAccessTokenCommand
    {
        public string ExpiredAccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
