using H.Necessaire.Runtime.Google.FirestoreDB.Core.Resources.Abstract;
using System;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Core.Resources
{
    internal class ConsumerIdentityGoogleFirestoreDbStorageResource
        : GoogleFirestoreDbStorageResourceBase<Guid, ConsumerIdentity, IDFilter<Guid>>
    {
    }
}
