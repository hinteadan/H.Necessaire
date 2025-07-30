using System;
using System.Threading.Tasks;

namespace H.Necessaire
{
    public interface ICanHandleNotificationMessages
    {
        event AsyncEventHandler<NotificationMessageReceivedEventArgs> OnNotificationMessageReceived;
    }

    public interface ImNotified : ICanHandleNotificationMessages
    {
        Task<OperationResult> RaiseNotificationReceived(NotificationMessage message);
    }

    public class NotificationMessageReceivedEventArgs : EventArgs
    {
        public NotificationMessageReceivedEventArgs(NotificationMessage notificationMessage)
        {
            NotificationMessage = notificationMessage;
        }

        public NotificationMessage NotificationMessage { get; }

        public static implicit operator NotificationMessageReceivedEventArgs(NotificationMessage message)
            => new NotificationMessageReceivedEventArgs(message);
    }
}
