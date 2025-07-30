using Android.Content;

namespace H.Necessaire.Runtime.MAUI.Platforms.Android
{
    public static class NotificationIntentExtensions
    {
        public static NotificationMessage ToNotificationMessage(this Intent intent)
        {
            if (intent is null)
                return null;

            NotificationMessage notificationMessage = (intent.GetStringExtra(nameof(NotificationMessage.Content)), intent.GetStringExtra(nameof(NotificationMessage.Subject)));
            int pendingIntentID = intent.GetIntExtra("PendingIntentID", -1);
            if (pendingIntentID > -1)
                notificationMessage.Notes = [$"{pendingIntentID}".NoteAs("PendingIntentID")];

            int messageID = intent.GetIntExtra("MessageID", -1);
            if (messageID > -1)
                notificationMessage.Notes = notificationMessage.Notes.Push($"{messageID}".NoteAs("MessageID"));

            string[] keys = intent.Extras.KeySet().Where(k => k.NotIn(nameof(NotificationMessage.Content), nameof(NotificationMessage.Subject), "PendingIntentID", "MessageID")).ToNonEmptyArray();
            if (!keys.IsEmpty())
            {
                List<Note> notes = new List<Note>(keys.Length);
                foreach (string key in keys)
                {
                    string value = intent.GetStringExtra(key);
                    notes.Add(new Note(key, value));
                }
                notificationMessage.Notes = notificationMessage.Notes.Push(notes);
            }

            return notificationMessage;
        }
    }
}
