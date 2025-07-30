namespace H.Necessaire
{
    public class NotificationMessage : EphemeralTypeBase
    {
        #region Construct
        public NotificationMessage() => DoNotExpire();
        #endregion

        public string Subject { get; set; }
        public string Content { get; set; }
        public NotificationMessageContentType ContentType { get; set; } = NotificationMessageContentType.Plain;
        public string Encoding { get; set; } = "utf-8";//Encoding.UTF8.WebName;
        public Note[] Notes { get; set; }

        public static implicit operator NotificationMessage(string message) => new NotificationMessage { Content = message };
        public static implicit operator NotificationMessage((string message, string subject) parts) => new NotificationMessage { Content = parts.message, Subject = parts.subject };
    }
}
