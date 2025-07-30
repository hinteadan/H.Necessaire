using Foundation;
using UserNotifications;

namespace H.Necessaire.Runtime.MAUI.Platforms.iOS
{
    public class IosNotificationReceiver : UNUserNotificationCenterDelegate
    {
        ImNotified notificationPoster;
        ImAnIosNotificationPoster notificationManager;
        public IosNotificationReceiver()
        {
            notificationPoster = IPlatformApplication.Current?.Services.GetService<IosNotificationService>();
            notificationManager = notificationPoster as ImAnIosNotificationPoster;
        }

        // Called if app is in the foreground.
        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            ProcessNotification(notification);

            var presentationOptions = (OperatingSystem.IsIOSVersionAtLeast(14))
                ? UNNotificationPresentationOptions.Banner | UNNotificationPresentationOptions.Sound
                : UNNotificationPresentationOptions.Alert | UNNotificationPresentationOptions.Sound
                ;

            completionHandler(presentationOptions);
        }

        // Called if app is in the background, or killed state.
        public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            if (response.IsDefaultAction)
                ProcessNotification(response.Notification);

            completionHandler();
        }

        void ProcessNotification(UNNotification notification)
        {
            NotificationMessage notificationMessage = (notification.Request.Content.Body, notification.Request.Content.Title);
            notificationMessage.Notes = MapNSDictionaryToNotificationMessageNotes(notification.Request.Content.UserInfo);

            notificationPoster?.RaiseNotificationReceived(notificationMessage);

            string messageID = notification.Request.Content.UserInfo[new NSString("MessageID")]?.ToString();
            if (!messageID.IsEmpty())
                notificationManager.ClearDisplayedNotifications(messageID);

        }

        static Note[] MapNSDictionaryToNotificationMessageNotes(NSDictionary notesDictionary)
        {
            if (notesDictionary is null || notesDictionary.Count == 0)
                return null;


            Note[] notes = new Note[notesDictionary.Count];
            int index = -1;
            foreach (KeyValuePair<NSObject, NSObject> entry in notesDictionary)
            {
                index++;
                notes[index] = (entry.Key.ToString(), entry.Value.ToString());
            }

            return notes;
        }
    }
}
