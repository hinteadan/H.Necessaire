namespace H.Necessaire.Runtime.MAUI.Platforms.iOS
{
    public interface ImAnIosNotificationPoster
    {
        void ClearDisplayedNotifications(params string[] notificationsIDs);
        void ClearAllDisplayedNotifications();
        void CancelScheduledNotifications(params string[] notificationsIDs);
        void CancelAllScheduledNotifications();
    }
}
