using H.Necessaire.Runtime.Google.FirestoreDB.Core.Resources.Abstract;
using System;
using System.Collections.Generic;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Core.Resources
{
    internal class QdActionResultGoogleFirestoreDbStorageResource
        : GoogleFirestoreDbStorageResourceBase<Guid, QdActionResult, QdActionResultFilter>
    {
        protected override IDictionary<string, Note> FilterToStoreMapping
            => new Dictionary<string, Note>() {
                { nameof(QdActionResultFilter.FromInclusive), nameof(QdActionResult.HappenedAt).NoteAs(">=") },
                { nameof(QdActionResultFilter.ToInclusive), nameof(QdActionResult.HappenedAt).NoteAs("<=") },
            };
    }
}
