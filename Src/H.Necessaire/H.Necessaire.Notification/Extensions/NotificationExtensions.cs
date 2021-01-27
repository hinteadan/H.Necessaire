using Newtonsoft.Json;

namespace H.Necessaire.Notification
{
    public static class NotificationExtensions
    {
        public static NotificationMessage ToJsonNotificationMessage(this object payload)
        {
            return
                JsonConvert
                    .SerializeObject(payload, Formatting.None)
                    .ToNotificationMessage(
                        subject: null,
                        NotificationMessageContentType.JSON
                    );
        }
    }
}
