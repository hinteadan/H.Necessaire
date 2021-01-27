namespace H.Necessaire
{
    public static class NotificationExtensions
    {
        public static NotificationAddress ToNotificationAddress(this string address, string name = null)
        {
            return new NotificationAddress { Address = address, Name = name };
        }

        public static NotificationEndpoint ToNotificationEndpoint(this NotificationAddress address, bool isPrivate = false, bool isOptional = false)
        {
            return new NotificationEndpoint { Address = address, IsOptional = isOptional, IsPrivate = isPrivate };
        }

        public static NotificationEndpoint ToNotificationEndpoint(this string address, string name = null, bool isPrivate = false, bool isOptional = false)
        {
            return address.ToNotificationAddress(name).ToNotificationEndpoint(isPrivate, isOptional);
        }

        public static NotificationMessage ToNotificationMessage(this string content, string subject = null, NotificationMessageContentType contentType = NotificationMessageContentType.Plain)
        {
            return new NotificationMessage { Content = content, ContentType = contentType, Subject = subject };
        }
    }
}
