using Google.Cloud.Firestore;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Core.Model.Querying
{
    public interface ImAFirestoreCriteria
    {
        Filter ToFilter();
    }
}
