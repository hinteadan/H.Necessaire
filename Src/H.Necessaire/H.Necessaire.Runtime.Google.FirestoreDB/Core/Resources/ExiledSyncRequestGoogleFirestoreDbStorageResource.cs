using H.Necessaire.Runtime.Google.FirestoreDB.Core.Resources.Abstract;
using System.Collections.Generic;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Core.Resources
{
    internal class ExiledSyncRequestGoogleFirestoreDbStorageResource
        : GoogleFirestoreDbStorageResourceBase<string, ExiledSyncRequest, SyncRequestFilter>
    {
        protected override IDictionary<string, Note> FilterToStoreMapping
            => new Dictionary<string, Note>() {
                { nameof(SyncRequestFilter.FromInclusive), nameof(ExiledSyncRequest.HappenedAt).NoteAs(">=") },
                { nameof(SyncRequestFilter.ToInclusive), nameof(ExiledSyncRequest.HappenedAt).NoteAs("<=") },
            };
    }
}
