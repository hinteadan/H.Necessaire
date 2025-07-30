using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Graphics.Drawable;

namespace H.Necessaire.Runtime.MAUI.Platforms.Android
{
    [ID("Device")]
    public class AndroidNotificationService : INotify, ImNotified, ImANotificationController, ImAnAndroidNotificationPoster
    {
        #region Construct
        readonly object trackingLocker = new object();
        readonly List<NotificationMessage> pendingNotificatons = new List<NotificationMessage>();
        const string defaultChannelID = "h-necessaire-default";
        const string defaultChannelName = "Default Notifications";
        const string defaultChannelDescription = "The default channel for notifications.";
        readonly string prefKeyForScheduledNotifications = null;
        readonly string channelID;
        readonly string channelName;
        readonly string channelDescription;
        bool isChannelInitialized = false;
        int messageID = 0;
        int pendingIntentID = 0;
        readonly NotificationManagerCompat androidNotificationManager;
        readonly Type mainActivityType;
        readonly Type notificationReceiverType;
        readonly MultiType<Bitmap, Icon> largeIcon;
        readonly MultiType<IconCompat, int> smallIcon;
        readonly AsyncEventRaiser<NotificationMessageReceivedEventArgs> notificationMessageReceivedRaiser;
        public AndroidNotificationService(Type mainActivityType, Type notificationReceiverType, MultiType<Bitmap, Icon> largeIcon = null, MultiType<IconCompat, int> smallIcon = null, string channelID = defaultChannelID, string channelName = defaultChannelName, string channelDescription = defaultChannelDescription)
        {
            this.channelID = channelID.IfEmpty(defaultChannelID);
            this.channelName = channelName.IfEmpty(defaultChannelName);
            this.channelDescription = channelDescription.IfEmpty(defaultChannelDescription);
            this.mainActivityType = mainActivityType;
            this.notificationReceiverType = notificationReceiverType;
            this.largeIcon = largeIcon;
            this.smallIcon = smallIcon;
            prefKeyForScheduledNotifications = $"SchNotifs::{channelID}";
            notificationMessageReceivedRaiser = new AsyncEventRaiser<NotificationMessageReceivedEventArgs>(this);
            androidNotificationManager = NotificationManagerCompat.From(Platform.AppContext);
        }
        #endregion

        public event AsyncEventHandler<NotificationMessageReceivedEventArgs> OnNotificationMessageReceived { add => notificationMessageReceivedRaiser.AddHandler(value); remove => notificationMessageReceivedRaiser.ZapHandler(value); }

        public async Task<OperationResult> RaiseNotificationReceived(NotificationMessage message)
        {
            await notificationMessageReceivedRaiser.Raise(message);
            return OperationResult.Win();
        }

        public Task<OperationResult> Send(NotificationMessage message, NotificationAddress from, params NotificationEndpoint[] to)
        {
            EnsureNotificationChannel();

            if (message.IsActive())
            {
                DisplayNotification(message);
                return OperationResult.Win().AsTask();
            }

            ScheduleNotification(message);

            return OperationResult.Win().AsTask();
        }

        public void Show(NotificationMessage notificationMessage)
        {
            int? scheduledIntentID = notificationMessage.Notes.Get("PendingIntentID").ParseToIntOrFallbackTo(null);
            if (scheduledIntentID is not null)
            {
                CancelScheduledNotifications(scheduledIntentID.Value);
            }

            DisplayNotification(notificationMessage);
        }

        public void ClearAllDisplayedNotifications()
        {
            androidNotificationManager.CancelAll();
        }

        public void ClearDisplayedNotifications(params int[] messagesIDs) { }

        public void CancelAllScheduledNotifications()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.UpsideDownCake)
                return;

            AlarmManager alarmManager = Platform.AppContext.GetSystemService(Context.AlarmService) as AlarmManager;
#pragma warning disable CA1416 // Validate platform compatibility
            alarmManager.CancelAll();
#pragma warning restore CA1416 // Validate platform compatibility
        }

        public void CancelScheduledNotifications(params int[] scheduledIntentIDs)
        {
            if (scheduledIntentIDs.IsEmpty())
                return;

            AlarmManager alarmManager = Platform.AppContext.GetSystemService(Context.AlarmService) as AlarmManager;

            foreach (int id in scheduledIntentIDs)
            {
                alarmManager.Cancel(BuildPendingIntentForScheduledNotificationCancellation(id));
                lock (trackingLocker)
                {
                    if (!pendingNotificatons.IsEmpty())
                    {
                        pendingNotificatons.RemoveAll(n => n.Notes.Get("PendingIntentID") == $"{id}");
                    }
                }
            }

            lock (trackingLocker)
            {
                if (pendingNotificatons.IsEmpty())
                    Preferences.Default.Remove(prefKeyForScheduledNotifications, channelID);
                else
                    Preferences.Default.Set(prefKeyForScheduledNotifications, SerializeNotifications(pendingNotificatons.ToArray()), channelID);
            }
        }

        public void ReSchedulePendingNotificationAfterDeviceRestartIfNecessary()
        {
            string savedNotifs = Preferences.Default.Get<string>(prefKeyForScheduledNotifications, null, channelID);
            NotificationMessage[] pendingNotifs = ParseNotifications(savedNotifs);
            Preferences.Default.Remove(prefKeyForScheduledNotifications, channelID);
            if (pendingNotifs.IsEmpty())
                return;

            foreach (NotificationMessage notif in pendingNotifs)
            {
                if (notif.ValidFrom.EnsureUtc() < DateTime.UtcNow.AddDays(-2))
                    continue;

                ScheduleNotification(notif);
            }
        }

        public Task<OperationResult> ClearDisplayedNotifications(params string[] notificationsIDs)
        {
            ClearDisplayedNotifications(notificationsIDs?.Select(x => x.ParseToIntOrFallbackTo(0).Value).Distinct().ToArray());

            return OperationResult.Win().AsTask();
        }

        Task<OperationResult> ImANotificationController.ClearAllDisplayedNotifications()
        {
            ClearAllDisplayedNotifications();

            return OperationResult.Win().AsTask();
        }

        public Task<OperationResult> CancelScheduledNotifications(params string[] notificationsIDs)
        {
            CancelScheduledNotifications(notificationsIDs?.Select(x => x.ParseToIntOrFallbackTo(0).Value).Distinct().ToArray());

            return OperationResult.Win().AsTask();
        }

        Task<OperationResult> ImANotificationController.CancelAllScheduledNotifications()
        {
            CancelAllScheduledNotifications();

            return OperationResult.Win().AsTask();
        }



        void DisplayNotification(NotificationMessage message)
        {
            Intent notificationDisplayIntent = BuildNotificationIntentForDisplay(message);

            PendingIntent pendingIntent = PendingIntent.GetActivity(Platform.AppContext, pendingIntentID++, notificationDisplayIntent, GetFlagsForPendingIntentForNotificationDisplay());

            NotificationCompat.Builder builder = new NotificationCompat.Builder(Platform.AppContext, channelID)
                .SetContentIntent(pendingIntent)
                .SetContentTitle(message.Subject)
                .SetContentText(message.Content)
#pragma warning disable CA1416 // Validate platform compatibility
                .AndIf(Build.VERSION.SdkInt >= BuildVersionCodes.M && largeIcon?.A != null, x => x.SetLargeIcon(largeIcon.A))
                .AndIf(Build.VERSION.SdkInt >= BuildVersionCodes.M && largeIcon?.B != null, x => x.SetLargeIcon(largeIcon.B))
#pragma warning restore CA1416 // Validate platform compatibility
                .AndIf(smallIcon?.A != null, x => x.SetSmallIcon(smallIcon.A))
                .AndIf(smallIcon?.B != null, x => x.SetSmallIcon(smallIcon.B))
                .SetAutoCancel(autoCancel: true)
                ;

            Notification notification = builder.Build();
            androidNotificationManager.Notify(messageID, notification);
        }

        void ScheduleNotification(NotificationMessage message)
        {
            InitScheduledNotificationsTrackingIfNecessary();

            Intent notificationIntent = BuildNotificationMessageIntentForScheduledNotification(message);
            DateTime notifyAt = message.ValidFrom.EnsureUtc();
            if (DateTime.UtcNow >= notifyAt)
                notifyAt = DateTime.UtcNow.AddSeconds(15);

            PendingIntent pendingIntent = PendingIntent.GetBroadcast(Platform.AppContext, pendingIntentID, notificationIntent, GetFlagsForForPendingIntentForScheduledNotification());

            AlarmManager alarmManager = Platform.AppContext.GetSystemService(Context.AlarmService) as AlarmManager;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
#pragma warning disable CA1416 // Validate platform compatibility
                alarmManager.SetAndAllowWhileIdle(AlarmType.RtcWakeup, ConvertNotificationTimeToAndroid(notifyAt), pendingIntent);
#pragma warning restore CA1416 // Validate platform compatibility
            else
                alarmManager.Set(AlarmType.RtcWakeup, ConvertNotificationTimeToAndroid(notifyAt), pendingIntent);

            lock (trackingLocker)
            {
                pendingNotificatons.Add(message);
                Preferences.Default.Set(prefKeyForScheduledNotifications, SerializeNotifications(pendingNotificatons.ToArray()), channelID);
            }
        }

        Intent BuildNotificationIntentForDisplay(NotificationMessage message)
        {
            messageID++;

            Intent intent = new Intent(Platform.AppContext, mainActivityType);
            intent.PutExtra(nameof(NotificationMessage.Subject), message.Subject);
            intent.PutExtra(nameof(NotificationMessage.Content), message.Content);
            intent.PutExtra("MessageID", messageID);
            CopyMessageNotesToIntent(message, intent);

            intent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);

            message.Notes = message.Notes.Push($"{messageID}".NoteAs("MessageID"));

            return intent;
        }

        Intent BuildNotificationMessageIntentForScheduledNotification(NotificationMessage message)
        {
            pendingIntentID++;

            Intent intent = new Intent(Platform.AppContext, notificationReceiverType);
            intent.PutExtra(nameof(NotificationMessage.Subject), message.Subject);
            intent.PutExtra(nameof(NotificationMessage.Content), message.Content);
            intent.PutExtra("PendingIntentID", pendingIntentID);
            CopyMessageNotesToIntent(message, intent);

            intent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);

            message.Notes = message.Notes.Push($"{pendingIntentID}".NoteAs("PendingIntentID"));

            return intent;
        }

        PendingIntent BuildPendingIntentForScheduledNotificationCancellation(int pendingIntentID)
        {
            Intent notificationIntent = new Intent(Platform.AppContext, notificationReceiverType);
            notificationIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);
            PendingIntent pendingIntent = PendingIntent.GetBroadcast(Platform.AppContext, pendingIntentID, notificationIntent, GetFlagsForForPendingIntentForScheduledNotification());
            return pendingIntent;
        }

        long ConvertNotificationTimeToAndroid(DateTime notifyTime)
        {
            DateTime utcTime = notifyTime;
            double epochDiff = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;
            long utcAlarmTime = utcTime.AddSeconds(-epochDiff).Ticks / 10000;
            return utcAlarmTime; // milliseconds
        }

        long ConvertTimeSpanToAndroid(TimeSpan timeSpan)
        {
            long androTicks = timeSpan.Ticks / 10000;
            return androTicks; // milliseconds
        }

        static PendingIntentFlags GetFlagsForPendingIntentForNotificationDisplay()
        {
            return (Build.VERSION.SdkInt >= BuildVersionCodes.S)
#pragma warning disable CA1416 // Validate platform compatibility
                            ? PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
#pragma warning restore CA1416 // Validate platform compatibility
                            : PendingIntentFlags.UpdateCurrent;
        }

        static PendingIntentFlags GetFlagsForForPendingIntentForScheduledNotification()
        {
            return (Build.VERSION.SdkInt >= BuildVersionCodes.S)
#pragma warning disable CA1416 // Validate platform compatibility
                            ? PendingIntentFlags.CancelCurrent | PendingIntentFlags.Immutable
#pragma warning restore CA1416 // Validate platform compatibility
                            : PendingIntentFlags.CancelCurrent;
        }

        string SerializeNotifications(params NotificationMessage[] notificationMessages)
        {
            if (notificationMessages.IsEmpty())
                return null;

            return string.Join("|::N::|", notificationMessages.Select(SerializeNotification));
        }
        string SerializeNotification(NotificationMessage notificationMessage) => string.Join("|H::H|", [notificationMessage.Subject, notificationMessage.Content, notificationMessage.ValidFromTicks, .. notificationMessage.Notes ?? []]);
        static NotificationMessage[] ParseNotifications(string value)
        {
            if (value.IsEmpty())
                return null;

            string[] notificationParts = value.Split("|::N::|", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (notificationParts.IsEmpty())
                return null;

            return notificationParts.Select(ParseNotification).ToNoNullsArray();
        }
        static NotificationMessage ParseNotification(string value)
        {
            var parts = value.Split("|H::H|", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.IsEmpty())
                return null;

            return new NotificationMessage
            {
                Subject = parts[0],
                Content = parts[1],
                ValidFromTicks = parts[2].ParseToLongOrFallbackTo(DateTime.UtcNow.AddSeconds(15).Ticks).Value,
            }.AndIf(parts.Length > 3, mess =>
            {
                mess.Notes = parts.Skip(3).Select(x => (Note)x).ToArray();
            });
        }

        void EnsureNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                return;

            if (isChannelInitialized)
                return;

            var channelNameJava = new Java.Lang.String(channelName);
#pragma warning disable CA1416 // Validate platform compatibility
            var channel = new NotificationChannel(channelID, channelNameJava, NotificationImportance.Default)
            {
                Description = channelDescription
            };
#pragma warning restore CA1416 // Validate platform compatibility
            // Register the channel
            NotificationManager manager = (NotificationManager)Platform.AppContext.GetSystemService(Context.NotificationService);
#pragma warning disable CA1416 // Validate platform compatibility
            manager.CreateNotificationChannel(channel);
#pragma warning restore CA1416 // Validate platform compatibility
            isChannelInitialized = true;
        }

        static void CopyMessageNotesToIntent(NotificationMessage message, Intent intent)
        {
            if (message.Notes.IsEmpty())
                return;

            foreach (var note in message.Notes)
            {
                intent.PutExtra(note.ID, note.Value);
            }
        }

        bool isScheduledNotificationsTrackingInitialized = false;
        void InitScheduledNotificationsTrackingIfNecessary()
        {
            if (isScheduledNotificationsTrackingInitialized) return;
            isScheduledNotificationsTrackingInitialized = true;

            string savedNotifs = Preferences.Default.Get<string>(prefKeyForScheduledNotifications, null, channelID);
            NotificationMessage[] pendingNotifs = ParseNotifications(savedNotifs);
            if (pendingNotifs.IsEmpty())
                return;
            pendingNotificatons.AddRange(pendingNotifs);
            pendingIntentID = pendingNotifs.Select(mess => mess.Notes.Get("PendingIntentID").ParseToIntOrFallbackTo(0).Value).Max();
        }
    }
}
