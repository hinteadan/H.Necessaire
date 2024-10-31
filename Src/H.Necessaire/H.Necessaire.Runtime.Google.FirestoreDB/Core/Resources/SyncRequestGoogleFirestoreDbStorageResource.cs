using H.Necessaire.Runtime.Google.FirestoreDB.Core.Resources.Abstract;
using System.Collections.Generic;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Core.Resources
{
    internal class SyncRequestGoogleFirestoreDbStorageResource
        : GoogleFirestoreDbStorageResourceBase<string, SyncRequest, SyncRequestFilter>
    {
        protected override IDictionary<string, Note> FilterToStoreMapping
            => new Dictionary<string, Note>() {
                { nameof(SyncRequestFilter.FromInclusive), nameof(SyncRequest.HappenedAt).NoteAs(">=") },
                { nameof(SyncRequestFilter.ToInclusive), nameof(SyncRequest.HappenedAt).NoteAs("<=") },
            };
    }
}
