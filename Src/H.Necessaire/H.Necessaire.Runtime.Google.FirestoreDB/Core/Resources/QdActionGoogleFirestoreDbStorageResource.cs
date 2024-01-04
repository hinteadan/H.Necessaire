using H.Necessaire.Runtime.Google.FirestoreDB.Core.Resources.Abstract;
using System;
using System.Collections.Generic;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Core.Resources
{
    internal class QdActionGoogleFirestoreDbStorageResource
        : GoogleFirestoreDbStorageResourceBase<Guid, QdAction, QdActionFilter>
    {
        protected override IDictionary<string, Note> FilterToStoreMapping
            => new Dictionary<string, Note>() {
                { nameof(QdActionFilter.FromInclusive), nameof(QdAction.QdAt).NoteAs(">=") },
                { nameof(QdActionFilter.ToInclusive), nameof(QdAction.QdAt).NoteAs("<=") },
            };
    }
}
