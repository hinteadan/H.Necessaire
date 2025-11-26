namespace H.Necessaire
{
    public class EphemeralType<TPayload> : EphemeralTypeBase, IEphemeralType<TPayload>
    {
        public TPayload Payload { get; set; }

        public static implicit operator EphemeralType<TPayload>(TPayload payload) => new EphemeralType<TPayload> { Payload = payload };
        public static implicit operator TPayload(EphemeralType<TPayload> ephemeralType) => ephemeralType is null ? default : ephemeralType.Payload;
    }
}
