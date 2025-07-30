using UserNotifications;

namespace H.Necessaire.Runtime.MAUI.Platforms.MacCatalyst
{
    public class MacNotificationPermissionsEnsurer : ImANotificationPermissionsEnsurer
    {
        public Task<OperationResult> RequestPermissions()
        {
            TaskCompletionSource<OperationResult> taskCompletionSource = new TaskCompletionSource<OperationResult>();

            // Request permission to use local notifications.
            UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert, (approved, err) =>
            {
                OperationResult result = approved ? OperationResult.Win() : "Notification Permissions not Granted";
                taskCompletionSource.SetResult(result);
            });

            return taskCompletionSource.Task;
        }
    }
}
