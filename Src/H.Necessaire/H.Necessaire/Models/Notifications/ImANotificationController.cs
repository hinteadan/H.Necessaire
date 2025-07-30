using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ImANotificationController
    {
        Task<OperationResult> ClearDisplayedNotifications(params string[] notificationsIDs);
        Task<OperationResult> ClearAllDisplayedNotifications();
        Task<OperationResult> CancelScheduledNotifications(params string[] notificationsIDs);
        Task<OperationResult> CancelAllScheduledNotifications();
    }
}
