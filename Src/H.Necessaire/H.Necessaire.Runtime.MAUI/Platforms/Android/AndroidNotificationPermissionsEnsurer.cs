namespace H.Necessaire.Runtime.MAUI.Platforms.Android
{
    public class AndroidNotificationPermissionsEnsurer : ImANotificationPermissionsEnsurer
    {
        public async Task<OperationResult> RequestPermissions()
        {
            PermissionStatus status = await Permissions.RequestAsync<NotificationPermission>();

            switch (status)
            {
                case PermissionStatus.Granted:
                    return OperationResult.Win();

                case PermissionStatus.Limited:
                case PermissionStatus.Restricted:
                    return OperationResult.Win().Warn("Limited");

                case PermissionStatus.Disabled:
                case PermissionStatus.Denied:
                case PermissionStatus.Unknown:
                default:
                    return "Notification Permissions not Granted";
            }
        }
    }
}
