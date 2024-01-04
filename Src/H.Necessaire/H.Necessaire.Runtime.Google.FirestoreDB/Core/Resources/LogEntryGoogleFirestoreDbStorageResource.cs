using H.Necessaire.Runtime.Google.FirestoreDB.Core.Resources.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Core.Resources
{
    internal class LogEntryGoogleFirestoreDbStorageResource
        : GoogleFirestoreDbStorageResourceBase<Guid, LogEntry, LogFilter>
    {
        protected override IDictionary<string, Note> FilterToStoreMapping
            => new Dictionary<string, Note>() {
                { nameof(LogFilter.FromInclusive), nameof(LogEntry.HappenedAt).NoteAs(">=") },
                { nameof(LogFilter.ToInclusive), nameof(LogEntry.HappenedAt).NoteAs("<=") },
            };
    }
}
