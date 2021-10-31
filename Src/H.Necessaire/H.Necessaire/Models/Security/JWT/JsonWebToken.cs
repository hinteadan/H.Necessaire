namespace H.Necessaire
{
    public class JsonWebToken<TPayload> where TPayload : JwtPayload
    {
        public JwtHeader Header { get; set; } = JwtHeader.Default;
        public TPayload Payload { get; set; }
        public string Signature { get; set; }
    }
}
