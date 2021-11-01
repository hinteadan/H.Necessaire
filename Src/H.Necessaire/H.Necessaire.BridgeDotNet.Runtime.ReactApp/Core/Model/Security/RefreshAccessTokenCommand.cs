namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class RefreshAccessTokenCommand
    {
        public string ExpiredAccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
