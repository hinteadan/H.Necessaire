namespace H.Necessaire
{
    public class NotificationEndpoint
    {
        public NotificationAddress Address { get; set; }
        public bool IsPrivate { get; set; } = false;
        public bool IsOptional { get; set; } = false;

        public Note[] Notes { get; set; }

        public static implicit operator NotificationEndpoint(string address) => new NotificationEndpoint { Address = address };
        public static implicit operator NotificationEndpoint(NotificationAddress address) => new NotificationEndpoint { Address = address };
    }
}
