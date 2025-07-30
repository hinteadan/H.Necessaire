using Android.App;
using Android.Content;

namespace H.Necessaire.Runtime.MAUI.Platforms.Android
{
    //[BroadcastReceiver(Enabled = true, Label = "Local Notifications Broadcast Receiver")]
    public abstract class AndroidNotificationReceiver : BroadcastReceiver
    {
        readonly ImAnAndroidNotificationPoster androidNotificationPoster;
        public AndroidNotificationReceiver(ImAnAndroidNotificationPoster notificationPoster)
        {
            this.androidNotificationPoster = notificationPoster;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent?.Extras is null || intent.Extras.IsEmpty)
                return;

            NotificationMessage notificationMessage = intent.ToNotificationMessage();

            androidNotificationPoster.Show(notificationMessage);
        }
    }

    [BroadcastReceiver(Enabled = true, Label = "Local Notifications Device Restart Receiver", Exported = true)]
    [IntentFilter([Intent.ActionBootCompleted])]
    public class AndroidNotificationDeviceRestartReceiver : BroadcastReceiver
    {
        readonly ImAnAndroidNotificationPoster androidNotificationPoster;
        public AndroidNotificationDeviceRestartReceiver()
        {
            androidNotificationPoster = HMauiApp.Default.DependencyRegistry.Build<ImAnAndroidNotificationPoster>("Device");
        }

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent?.Action != Intent.ActionBootCompleted)
                return;

            androidNotificationPoster.ReSchedulePendingNotificationAfterDeviceRestartIfNecessary();
            return;
        }
    }
}
