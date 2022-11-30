namespace H.Necessaire
{
    public class EphemeralType<TPayload> : EphemeralTypeBase, IEphemeralType<TPayload>
    {
        public TPayload Payload { get; set; }
    }
}
