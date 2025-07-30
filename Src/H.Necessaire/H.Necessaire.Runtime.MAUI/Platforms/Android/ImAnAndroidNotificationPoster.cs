namespace H.Necessaire.Runtime.MAUI.Platforms.Android
{
    public interface ImAnAndroidNotificationPoster
    {
        void Show(NotificationMessage notificationMessage);
        void ClearAllDisplayedNotifications();
        void ClearDisplayedNotifications(params int[] messagesIDs);
        void CancelAllScheduledNotifications();
        void CancelScheduledNotifications(params int[] scheduledIntentIDs);
        void ReSchedulePendingNotificationAfterDeviceRestartIfNecessary();
    }
}
