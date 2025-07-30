using UserNotifications;

namespace H.Necessaire.Runtime.MAUI.Platforms.iOS
{
    public class IosNotificationPermissionsEnsurer : ImANotificationPermissionsEnsurer
    {
        public Task<OperationResult> RequestPermissions()
        {
            TaskCompletionSource<OperationResult> taskCompletionSource = new TaskCompletionSource<OperationResult>();

            // Request permission to use local notifications.
            UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound, (approved, err) =>
            {
                OperationResult result = approved ? OperationResult.Win() : "Notification Permissions not Granted";
                taskCompletionSource.SetResult(result);
            });

            return taskCompletionSource.Task;
        }
    }
}
