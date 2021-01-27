namespace H.Necessaire
{
    public class NotificationEndpoint
    {
        public NotificationAddress Address { get; set; }
        public bool IsPrivate { get; set; } = false;
        public bool IsOptional { get; set; } = false;
    }
}
