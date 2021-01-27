namespace H.Necessaire
{
    public class NotificationMessage
    {
        public string Subject { get; set; }
        public string Content { get; set; }
        public NotificationMessageContentType ContentType { get; set; } = NotificationMessageContentType.Plain;
    }
}
