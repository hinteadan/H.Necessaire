using Foundation;
using UserNotifications;

namespace H.Necessaire.Runtime.MAUI.Platforms.iOS
{
    [ID("Device")]
    public class IosNotificationService : ImADependency, INotify, ImNotified, ImANotificationController, ImAnIosNotificationPoster
    {
        #region Construct
        readonly AsyncEventRaiser<NotificationMessageReceivedEventArgs> notificationMessageReceivedRaiser;
        public IosNotificationService()
        {
            notificationMessageReceivedRaiser = new AsyncEventRaiser<NotificationMessageReceivedEventArgs>(this);
        }
        ImANotificationPermissionsEnsurer permissionsEnsurer;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            permissionsEnsurer = dependencyProvider.Get<ImANotificationPermissionsEnsurer>();

            // Create a UNUserNotificationCenterDelegate to handle incoming messages.
            UNUserNotificationCenter.Current.Delegate = new IosNotificationReceiver();
        }
        #endregion
        public event AsyncEventHandler<NotificationMessageReceivedEventArgs> OnNotificationMessageReceived { add => notificationMessageReceivedRaiser.AddHandler(value); remove => notificationMessageReceivedRaiser.ZapHandler(value); }

        public async Task<OperationResult> Send(NotificationMessage message, NotificationAddress from, params NotificationEndpoint[] to)
        {
            if (!(await permissionsEnsurer.RequestPermissions()).Ref(out var permRes))
                return permRes;

            string messageID = Guid.NewGuid().ToString();
            message.Notes = message.Notes.Push($"{messageID}".NoteAs("MessageID"));
            var content = new UNMutableNotificationContent
            {
                Title = message.Subject,
                Subtitle = "",
                Body = message.Content,
                Badge = 1,
                UserInfo = MapNotificationMessageNotesToNSDictionary(message),
                Sound = UNNotificationSound.Default,
            };

            UNNotificationTrigger trigger;

            if (message.IsActive())
                trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(0.25, false);
            else
                trigger = UNCalendarNotificationTrigger.CreateTrigger(GetNSDateComponents(message.ValidFrom.EnsureUtc().ToLocalTime()), false);

            var request = UNNotificationRequest.FromIdentifier($"{messageID}", content, trigger);

            TaskCompletionSource<OperationResult> taskCompletionSource = new TaskCompletionSource<OperationResult>();

            UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
            {
                if (err != null)
                    taskCompletionSource.SetResult($"Failed to schedule notification: {err}");

                taskCompletionSource.SetResult(OperationResult.Win());
            });

            return await taskCompletionSource.Task;
        }

        public async Task<OperationResult> RaiseNotificationReceived(NotificationMessage message)
        {
            await notificationMessageReceivedRaiser.Raise(message);
            return OperationResult.Win();
        }

        public void CancelScheduledNotifications(params string[] notificationsIDs)
        {
            if (notificationsIDs.IsEmpty())
                return;

            UNUserNotificationCenter.Current.RemovePendingNotificationRequests(notificationsIDs);
        }

        public void ClearDisplayedNotifications(params string[] notificationsIDs)
        {
            if (notificationsIDs.IsEmpty())
                return;

            UNUserNotificationCenter.Current.RemoveDeliveredNotifications(notificationsIDs);
        }

        public void ClearAllDisplayedNotifications()
        {
            UNUserNotificationCenter.Current.RemoveAllDeliveredNotifications();
        }

        public void CancelAllScheduledNotifications()
        {
            UNUserNotificationCenter.Current.RemoveAllPendingNotificationRequests();
        }


        Task<OperationResult> ImANotificationController.ClearDisplayedNotifications(params string[] notificationsIDs)
        {
            ClearDisplayedNotifications(notificationsIDs);
            return OperationResult.Win().AsTask();
        }

        Task<OperationResult> ImANotificationController.ClearAllDisplayedNotifications()
        {
            ClearAllDisplayedNotifications();
            return OperationResult.Win().AsTask();
        }

        Task<OperationResult> ImANotificationController.CancelScheduledNotifications(params string[] notificationsIDs)
        {
            CancelScheduledNotifications(notificationsIDs);
            return OperationResult.Win().AsTask();
        }

        Task<OperationResult> ImANotificationController.CancelAllScheduledNotifications()
        {
            CancelAllScheduledNotifications();
            return OperationResult.Win().AsTask();
        }



        NSDateComponents GetNSDateComponents(DateTime dateTime)
        {
            return new NSDateComponents
            {
                Month = dateTime.Month,
                Day = dateTime.Day,
                Year = dateTime.Year,
                Hour = dateTime.Hour,
                Minute = dateTime.Minute,
                Second = dateTime.Second
            };
        }

        static NSDictionary MapNotificationMessageNotesToNSDictionary(NotificationMessage message)
        {
            if (message.Notes.IsEmpty())
                return null;

            return NSDictionary.FromObjectsAndKeys(
                message.Notes.Select(n => n.Value).ToArray(),
                message.Notes.Select(n => n.ID).ToArray()
            );
        }
    }
}
