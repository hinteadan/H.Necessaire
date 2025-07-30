namespace H.Necessaire.Runtime.MAUI.Platforms.MacCatalyst
{
    public interface ImAMacNotificationPoster
    {
        void ClearDisplayedNotifications(params string[] notificationsIDs);
        void ClearAllDisplayedNotifications();
        void CancelScheduledNotifications(params string[] notificationsIDs);
        void CancelAllScheduledNotifications();
    }
}
