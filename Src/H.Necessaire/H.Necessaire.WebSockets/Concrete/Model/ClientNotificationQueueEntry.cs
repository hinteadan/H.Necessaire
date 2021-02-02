namespace H.Necessaire.WebSockets.Concrete.Model
{
    class ClientNotificationQueueEntry
    {
        public static readonly ClientNotificationQueueEntry None = new ClientNotificationQueueEntry();

        public NotificationMessage Message { get; set; }
        public NotificationAddress From { get; set; }
        public NotificationEndpoint[] To { get; set; } = new NotificationEndpoint[0];
    }
}
